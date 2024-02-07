# Use the Microsoft's official .NET runtime image from Docker Hub.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the SDK image to build the project files.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TimestampLogger.csproj", "./"]
RUN dotnet restore "./TimestampLogger.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TimestampLogger.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimestampLogger.csproj" -c Release -o /app/publish

# Copy the build output to the runtime image.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY appsettings.json .
ENTRYPOINT ["dotnet", "TimestampLogger.dll"]
