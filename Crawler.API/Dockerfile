#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Crawler.API/Crawler.API.csproj", "Crawler.API/"]
COPY ["Crawler.Infra.RabbitMq/Crawler.Infra.RabbitMq.csproj", "Crawler.Infra.RabbitMq/"]
RUN dotnet restore "Crawler.API/Crawler.API.csproj"
COPY . .
WORKDIR "/src/Crawler.API"
RUN dotnet build "Crawler.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Crawler.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crawler.API.dll"]