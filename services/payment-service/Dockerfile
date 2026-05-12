# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY payment-service.sln ./
COPY QuickBite.Payment.API/QuickBite.Payment.API.csproj QuickBite.Payment.API/
COPY QuickBite.Payment.Application/QuickBite.Payment.Application.csproj QuickBite.Payment.Application/
COPY QuickBite.Payment.Domain/QuickBite.Payment.Domain.csproj QuickBite.Payment.Domain/
COPY QuickBite.Payment.Infrastructure/QuickBite.Payment.Infrastructure.csproj QuickBite.Payment.Infrastructure/

# Restore packages
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish
RUN dotnet publish QuickBite.Payment.API/QuickBite.Payment.API.csproj \
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

ENTRYPOINT ["dotnet", "QuickBite.Payment.API.dll"]
