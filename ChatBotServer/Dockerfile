FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChatBotServer/ChatBotServer.API/ChatBotServer.API.csproj", "ChatBotServer.API/"]
COPY ["ChatBotServer/ChatBotServer.Application/ChatBotServer.Application.csproj", "ChatBotServer.Application/"]
COPY ["ChatBotServer/ChatBotServer.Domain/ChatBotServer.Domain.csproj", "ChatBotServer.Domain/"]
COPY ["ChatBotServer/ChatBotServer.Infrastructure/ChatBotServer.Infrastructure.csproj", "ChatBotServer.Infrastructure/"]
RUN dotnet restore "ChatBotServer.API/ChatBotServer.API.csproj"
COPY . .
WORKDIR "/src/ChatBotServer/ChatBotServer.API"
RUN dotnet build "./ChatBotServer.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChatBotServer.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatBotServer.API.dll"]
