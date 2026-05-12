# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY delivery-agent-service.sln ./
COPY QuickBite.DeliveryAgent.API/QuickBite.DeliveryAgent.API.csproj QuickBite.DeliveryAgent.API/
COPY QuickBite.DeliveryAgent.Application/QuickBite.DeliveryAgent.Application.csproj QuickBite.DeliveryAgent.Application/
COPY QuickBite.DeliveryAgent.Domain/QuickBite.DeliveryAgent.Domain.csproj QuickBite.DeliveryAgent.Domain/
COPY QuickBite.DeliveryAgent.Infrastructure/QuickBite.DeliveryAgent.Infrastructure.csproj QuickBite.DeliveryAgent.Infrastructure/

# Restore packages
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish
RUN dotnet publish QuickBite.DeliveryAgent.API/QuickBite.DeliveryAgent.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render sets PORT env variable
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}

EXPOSE 8080

ENTRYPOINT ["dotnet", "QuickBite.DeliveryAgent.API.dll"]
