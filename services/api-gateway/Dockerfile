# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY api-gateway.sln ./
COPY QuickBite.ApiGateway/QuickBite.ApiGateway.csproj QuickBite.ApiGateway/

RUN dotnet restore

COPY . .

RUN dotnet publish QuickBite.ApiGateway/QuickBite.ApiGateway.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "QuickBite.ApiGateway.dll"]