FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UserProfileService/FitnessApp.UserProfileService.csproj", "UserProfileService/"]
COPY ["FitnessApp.Shared/FitnessApp.Shared.csproj", "FitnessApp.Shared/"]
RUN dotnet restore "UserProfileService/FitnessApp.UserProfileService.csproj"
COPY . .
WORKDIR "/src/UserProfileService"
RUN dotnet build "./FitnessApp.UserProfileService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FitnessApp.UserProfileService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitnessApp.UserProfileService.dll"]
