# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY order-service.sln ./
COPY QuickBite.Order.API/QuickBite.Order.API.csproj QuickBite.Order.API/
COPY QuickBite.Order.Application/QuickBite.Order.Application.csproj QuickBite.Order.Application/
COPY QuickBite.Order.Domain/QuickBite.Order.Domain.csproj QuickBite.Order.Domain/
COPY QuickBite.Order.Infrastructure/QuickBite.Order.Infrastructure.csproj QuickBite.Order.Infrastructure/

# Restore packages
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish
RUN dotnet publish QuickBite.Order.API/QuickBite.Order.API.csproj \
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

ENTRYPOINT ["dotnet", "QuickBite.Order.API.dll"]
