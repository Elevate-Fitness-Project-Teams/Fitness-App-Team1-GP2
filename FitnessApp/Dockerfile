# Base image (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

USER $APP_UID

WORKDIR /app

EXPOSE 8080
EXPOSE 8081


# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src


# Copy project file first (for Docker cache)
COPY ["FitnessApp.csproj", "./"]

# Restore NuGet packages
RUN dotnet restore "./FitnessApp.csproj"


# Copy the rest of the source code
COPY . .


# Build application
RUN dotnet build "./FitnessApp.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build


# Publish stage
FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./FitnessApp.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false



# Final runtime image
FROM base AS final

WORKDIR /app


# Copy published files from publish stage
COPY --from=publish /app/publish .


# Start application
ENTRYPOINT ["dotnet", "FitnessApp.dll"]