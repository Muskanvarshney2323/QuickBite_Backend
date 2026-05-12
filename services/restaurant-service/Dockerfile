# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY restaurant-service.sln ./
COPY QuickBite.Restaurant.API/QuickBite.Restaurant.API.csproj QuickBite.Restaurant.API/
COPY QuickBite.Restaurant.Application/QuickBite.Restaurant.Application.csproj QuickBite.Restaurant.Application/
COPY QuickBite.Restaurant.Domain/QuickBite.Restaurant.Domain.csproj QuickBite.Restaurant.Domain/
COPY QuickBite.Restaurant.Infrastructure/QuickBite.Restaurant.Infrastructure.csproj QuickBite.Restaurant.Infrastructure/

# Restore packages
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish
RUN dotnet publish QuickBite.Restaurant.API/QuickBite.Restaurant.API.csproj \
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

ENTRYPOINT ["dotnet", "QuickBite.Restaurant.API.dll"]
