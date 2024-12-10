FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build

# Restoring
WORKDIR /App
COPY Directory.Packages.props .
WORKDIR /App/src
COPY src/Directory.Packages.props .
ARG BUILD_CONFIGURATION=Release
COPY src/Microservices/Identity ./Microservices/Identity
COPY src/Common ./Common
WORKDIR /App/src/Microservices/Identity/Api/MyKitchen.Microservices.Identity.Api.Rest
RUN dotnet restore "MyKitchen.Microservices.Identity.Api.Rest.csproj"

# Building
WORKDIR /App
COPY . .
WORKDIR /App/src/Microservices/Identity/Api/MyKitchen.Microservices.Identity.Api.Rest
RUN dotnet build "MyKitchen.Microservices.Identity.Api.Rest.csproj" -c ${BUILD_CONFIGURATION} --no-restore

# Publishing
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /App/src/Microservices/Identity/Api/MyKitchen.Microservices.Identity.Api.Rest
RUN dotnet publish "MyKitchen.Microservices.Identity.Api.Rest.csproj" -c ${BUILD_CONFIGURATION} --no-build -o /App/publish /p:UseAppHost=false

# Running
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff AS runtime
WORKDIR /App
COPY --from=publish /App/publish .
EXPOSE 5000
EXPOSE 5001
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "MyKitchen.Microservices.Identity.Api.Rest.dll"]
