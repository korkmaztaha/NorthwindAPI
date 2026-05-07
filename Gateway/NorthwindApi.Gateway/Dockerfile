FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Gateway/NorthwindApi.Gateway/NorthwindApi.Gateway.csproj", "Gateway/NorthwindApi.Gateway/"]

RUN dotnet restore "Gateway/NorthwindApi.Gateway/NorthwindApi.Gateway.csproj"

COPY . .

WORKDIR "/src/Gateway/NorthwindApi.Gateway"
RUN dotnet build "NorthwindApi.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NorthwindApi.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NorthwindApi.Gateway.dll"]