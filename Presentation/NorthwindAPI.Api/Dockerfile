FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Presentation/NorthwindAPI.Api/NorthwindAPI.Api.csproj", "Presentation/NorthwindAPI.Api/"]
COPY ["Core/NorthwindApi.Application/NorthwindApi.Application.csproj", "Core/NorthwindApi.Application/"]
COPY ["Core/NorthwindApi.Domain/NorthwindApi.Domain.csproj", "Core/NorthwindApi.Domain/"]
COPY ["Infrastructure/NorthwindApi.Persistence/NorthwindApi.Persistence.csproj", "Infrastructure/NorthwindApi.Persistence/"]

RUN dotnet restore "Presentation/NorthwindAPI.Api/NorthwindAPI.Api.csproj"

COPY . .

WORKDIR "/src/Presentation/NorthwindAPI.Api"
RUN dotnet build "NorthwindAPI.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NorthwindAPI.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NorthwindAPI.Api.dll"]