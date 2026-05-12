namespace QuickBite.Payment.Domain.Enums;

/// <summary>
/// Direction of a wallet statement entry: a credit (deposit/top-up)
/// or a debit (spend/payment).
/// </summary>
public enum WalletStatementType
{
    DEPOSIT = 0,
    DEBIT = 1
}
