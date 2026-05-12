using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuickBite.Order.Application.Interfaces;
using QuickBite.Order.Domain.Enums;

namespace QuickBite.Order.Infrastructure.ExternalServices;

/// <summary>
/// HTTP adapter that connects Order-Service to Payment-Service.
/// It creates a payment when an order is placed and triggers a refund when an order is cancelled.
/// Handles duplicate payment requests gracefully by returning existing payment records.
/// </summary>
public class HttpPaymentGateway : IPaymentGateway
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpPaymentGateway> _logger;

    public HttpPaymentGateway(HttpClient httpClient, IConfiguration configuration, ILogger<HttpPaymentGateway> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var baseUrl = configuration["PaymentService:BaseUrl"] ?? "http://localhost:5114";
        _httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        _logger.LogInformation("HttpPaymentGateway initialized with base URL: {BaseUrl}", _httpClient.BaseAddress);
    }

    public async Task<bool> ProcessPaymentAsync(Guid orderId, Guid customerId, decimal amount, PaymentMode mode)
    {
        // Cash on delivery does not need an online payment record before confirmation.
        if (mode == PaymentMode.CASH_ON_DELIVERY)
        {
            _logger.LogInformation("Payment skipped for order {OrderId}: Cash on Delivery selected", orderId);
            return true;
        }

        try
        {
            // Prepare payment request matching ProcessPaymentRequestDto structure
            var request = new
            {
                OrderId = orderId,
                CustomerId = customerId,
                Amount = amount,
                Mode = mode,  // Send as enum value; JsonStringEnumConverter on both sides handles serialization
                Currency = "INR"
            };

            _logger.LogInformation("Sending payment request to Payment-Service for order {OrderId}: amount={Amount} mode={Mode}",
                orderId, amount, mode);

            var response = await _httpClient.PostAsJsonAsync("api/v1/payments/process", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Payment-Service failed for order {OrderId}. Status={StatusCode}. Response={Error}",
                    orderId, response.StatusCode, error);

                // Log specific status codes for debugging
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    _logger.LogError("Payment request had invalid format. OrderId={OrderId}, Amount={Amount}, Mode={Mode}",
                        orderId, amount, mode);
                }

                return false;
            }

            _logger.LogInformation("Payment processed successfully for order {OrderId}", orderId);
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while processing payment for order {OrderId}. Message={Message}",
                orderId, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while processing payment for order {OrderId}",
                orderId);
            return false;
        }
    }

    public async Task<bool> TriggerRefundAsync(Guid orderId, decimal amount, PaymentMode mode)
    {
        // Cash on delivery has no gateway charge to refund.
        if (mode == PaymentMode.CASH_ON_DELIVERY)
        {
            _logger.LogInformation("Refund skipped for order {OrderId}: Cash on Delivery mode", orderId);
            return true;
        }

        try
        {
            _logger.LogInformation("Fetching payment record for order {OrderId} to trigger refund", orderId);

            // First, get the payment record to find the payment ID
            var paymentResponse = await _httpClient.GetAsync($"api/v1/payments/byOrder/{orderId}");

            if (!paymentResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("Payment record not found for order {OrderId}; refund skipped. Status={StatusCode}",
                    orderId, paymentResponse.StatusCode);
                return false;
            }

            var payment = await paymentResponse.Content.ReadFromJsonAsync<PaymentLookupResponse>();
            if (payment?.PaymentId == Guid.Empty)
            {
                _logger.LogWarning("Payment response for order {OrderId} did not contain a valid PaymentId", orderId);
                return false;
            }

            _logger.LogInformation("Triggering refund for payment {PaymentId} (order {OrderId}). Refund amount={Amount}",
                payment!.PaymentId, orderId, amount);

            // Trigger refund endpoint
            var refundResponse = await _httpClient.PostAsJsonAsync($"api/v1/payments/{payment!.PaymentId}/refund", new
            {
                Reason = "Order cancelled by customer"
            });

            if (!refundResponse.IsSuccessStatusCode)
            {
                var error = await refundResponse.Content.ReadAsStringAsync();
                _logger.LogError("Refund failed for order {OrderId}. Status={StatusCode}. Response={Error}",
                    orderId, refundResponse.StatusCode, error);
                return false;
            }

            _logger.LogInformation("Refund completed successfully for order {OrderId}", orderId);
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while triggering refund for order {OrderId}",
                orderId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while triggering refund for order {OrderId}",
                orderId);
            return false;
        }
    }

    private sealed class PaymentLookupResponse
    {
        public Guid PaymentId { get; set; }
    }
}
