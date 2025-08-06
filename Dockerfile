# ===== Base runtime image =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ===== Build stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["MangaAPI.csproj", "./"]
RUN dotnet restore "MangaAPI.csproj"

# Copy the rest of the source code
COPY . .
RUN dotnet build "MangaAPI.csproj" -c Release -o /app/build

# ===== Publish stage =====
FROM build AS publish
RUN dotnet publish "MangaAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===== Final stage =====
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entry point to run the app
ENTRYPOINT ["dotnet", "MangaAPI.dll"]
