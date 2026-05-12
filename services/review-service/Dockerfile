# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY review-service.sln ./
COPY QuickBite.Review.API/QuickBite.Review.API.csproj QuickBite.Review.API/
COPY QuickBite.Review.Application/QuickBite.Review.Application.csproj QuickBite.Review.Application/
COPY QuickBite.Review.Domain/QuickBite.Review.Domain.csproj QuickBite.Review.Domain/
COPY QuickBite.Review.Infrastructure/QuickBite.Review.Infrastructure.csproj QuickBite.Review.Infrastructure/

# Restore packages
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish
RUN dotnet publish QuickBite.Review.API/QuickBite.Review.API.csproj \
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

ENTRYPOINT ["dotnet", "QuickBite.Review.API.dll"]
