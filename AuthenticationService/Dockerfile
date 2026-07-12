# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy project files first for layer caching
COPY ["AuthenticationService/FitnessApp.AuthenticationService.csproj", "AuthenticationService/"]
COPY ["FitnessApp.Shared/FitnessApp.Shared.csproj", "FitnessApp.Shared/"]
RUN dotnet restore "./AuthenticationService/FitnessApp.AuthenticationService.csproj"
# Copy the rest of the source code
COPY ["AuthenticationService/", "AuthenticationService/"]
COPY ["FitnessApp.Shared/", "FitnessApp.Shared/"]
WORKDIR "/src/AuthenticationService"
RUN dotnet build "./FitnessApp.AuthenticationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FitnessApp.AuthenticationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitnessApp.AuthenticationService.dll"]