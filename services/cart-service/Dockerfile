# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY cart-service.sln ./
COPY QuickBite.Cart.API/QuickBite.Cart.API.csproj QuickBite.Cart.API/
COPY QuickBite.Cart.Application/QuickBite.Cart.Application.csproj QuickBite.Cart.Application/
COPY QuickBite.Cart.Domain/QuickBite.Cart.Domain.csproj QuickBite.Cart.Domain/
COPY QuickBite.Cart.Infrastructure/QuickBite.Cart.Infrastructure.csproj QuickBite.Cart.Infrastructure/

# Restore packages
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish
RUN dotnet publish QuickBite.Cart.API/QuickBite.Cart.API.csproj \
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

ENTRYPOINT ["dotnet", "QuickBite.Cart.API.dll"]
