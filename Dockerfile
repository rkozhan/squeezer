# Stage 1: Build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY Squeezer.sln ./

# Copy the project files for both the server and client
COPY Squeezer/Squeezer/*.csproj ./Squeezer/Squeezer/
COPY Squeezer/Squeezer.Client/*.csproj ./Squeezer/Squeezer.Client/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the code and build the projects
COPY Squeezer/. ./Squeezer/
WORKDIR /src/Squeezer/Squeezer
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Serve the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port for the application
EXPOSE 80
ENTRYPOINT ["dotnet", "Squeezer.dll"]
