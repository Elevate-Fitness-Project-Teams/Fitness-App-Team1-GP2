FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FitnessApp/WorkoutService.csproj", "FitnessApp/"]
COPY ["FitnessApp.Shared/FitnessApp.Shared.csproj", "FitnessApp.Shared/"]
RUN dotnet restore "FitnessApp/WorkoutService.csproj"
COPY . .
WORKDIR "/src/FitnessApp"
RUN dotnet build "./WorkoutService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WorkoutService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkoutService.dll"]
