FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/dotnet-cache/DotNetCacheAPI.csproj", "dotnet-cache/"]
COPY ["src/Directory.Packages.props", "dotnet-cache/"]
RUN dotnet restore "dotnet-cache/DotNetCacheAPI.csproj"
COPY src/ .
WORKDIR "/src/dotnet-cache"
RUN dotnet build "DotNetCacheAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DotNetCacheAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "DotNetCacheAPI.dll"]