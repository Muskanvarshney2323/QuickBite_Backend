# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY notification-service.sln ./
COPY QuickBite.Notification.API/QuickBite.Notification.API.csproj QuickBite.Notification.API/
COPY QuickBite.Notification.Application/QuickBite.Notification.Application.csproj QuickBite.Notification.Application/
COPY QuickBite.Notification.Domain/QuickBite.Notification.Domain.csproj QuickBite.Notification.Domain/
COPY QuickBite.Notification.Infrastructure/QuickBite.Notification.Infrastructure.csproj QuickBite.Notification.Infrastructure/

# Restore packages
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish
RUN dotnet publish QuickBite.Notification.API/QuickBite.Notification.API.csproj \
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

ENTRYPOINT ["dotnet", "QuickBite.Notification.API.dll"]
