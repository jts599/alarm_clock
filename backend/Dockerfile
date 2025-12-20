FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Install network utilities for WiFi management
RUN apt-get update && DEBIAN_FRONTEND=noninteractive apt-get install -y \
    iproute2 \
    wireless-tools \
    sudo \
    tzdata \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy LifxNet source project first (dependency)
COPY ["LifxNet-Source/src/LifxNet/LifxNet.csproj", "LifxNet-Source/src/LifxNet/"]

# Copy backend project file
COPY ["backend/backend.csproj", "backend/"]

# Restore dependencies
RUN dotnet restore "backend/backend.csproj"

# Copy remaining source files
COPY ["LifxNet-Source/src/LifxNet/", "LifxNet-Source/src/LifxNet/"]
COPY ["backend/", "backend/"]

# Build
WORKDIR /src/backend
RUN dotnet build "backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend.dll"]