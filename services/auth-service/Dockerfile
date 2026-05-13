# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY QuickBite.Auth.API/QuickBite.Auth.API.csproj QuickBite.Auth.API/
COPY QuickBite.Auth.Application/QuickBite.Auth.Application.csproj QuickBite.Auth.Application/
COPY QuickBite.Auth.Domain/QuickBite.Auth.Domain.csproj QuickBite.Auth.Domain/
COPY QuickBite.Auth.Infrastructure/QuickBite.Auth.Infrastructure.csproj QuickBite.Auth.Infrastructure/

# Restore packages
RUN dotnet restore QuickBite.Auth.API/QuickBite.Auth.API.csproj

# Copy all source files
COPY . .

# Build and publish
RUN dotnet publish QuickBite.Auth.API/QuickBite.Auth.API.csproj \
    -c Release \
    -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render sets PORT env variable
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}

EXPOSE 8080

ENTRYPOINT ["dotnet", "QuickBite.Auth.API.dll"]
