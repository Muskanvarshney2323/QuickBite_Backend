# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY menu-service.sln ./
COPY QuickBite.Menu.API/QuickBite.Menu.API.csproj QuickBite.Menu.API/
COPY QuickBite.Menu.Application/QuickBite.Menu.Application.csproj QuickBite.Menu.Application/
COPY QuickBite.Menu.Domain/QuickBite.Menu.Domain.csproj QuickBite.Menu.Domain/
COPY QuickBite.Menu.Infrastructure/QuickBite.Menu.Infrastructure.csproj QuickBite.Menu.Infrastructure/

# Restore packages
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish
RUN dotnet publish QuickBite.Menu.API/QuickBite.Menu.API.csproj \
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

ENTRYPOINT ["dotnet", "QuickBite.Menu.API.dll"]
