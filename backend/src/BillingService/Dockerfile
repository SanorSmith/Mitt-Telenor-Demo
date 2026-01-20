FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/BillingService/BillingService.csproj", "src/BillingService/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
RUN dotnet restore "src/BillingService/BillingService.csproj"
COPY . .
WORKDIR "/src/src/BillingService"
RUN dotnet build "BillingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BillingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BillingService.dll"]
