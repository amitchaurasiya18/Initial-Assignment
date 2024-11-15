FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5206
ENV ASPNETCORE_ENVIRONMENT=Development

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Debug
WORKDIR /src
COPY ["src/SchoolAPI/SchoolAPI.csproj", "src/SchoolAPI/"]
RUN dotnet restore "src/SchoolAPI/SchoolAPI.csproj"
COPY . .
WORKDIR "/src/src/SchoolAPI"
RUN dotnet build "SchoolAPI.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Debug
RUN dotnet publish "SchoolAPI.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5206
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SchoolAPI.dll"]
