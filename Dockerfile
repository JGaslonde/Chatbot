# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Chatbot.sln", "./"]
COPY ["Chatbot.API/Chatbot.API.csproj", "Chatbot.API/"]
COPY ["Chatbot/Chatbot.csproj", "Chatbot/"]

# Restore dependencies
RUN dotnet restore "Chatbot.API/Chatbot.API.csproj"

# Copy the rest of the files
COPY . .

# Build and publish the application
WORKDIR "/src/Chatbot.API"
RUN dotnet build "Chatbot.API.csproj" -c Release -o /app/build
RUN dotnet publish "Chatbot.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install SQLite (for runtime database operations)
RUN apt-get update && apt-get install -y sqlite3 && rm -rf /var/lib/apt/lists/*

# Copy published files from build stage
COPY --from=build /app/publish .

# Create directory for database
RUN mkdir -p /app/data

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5089
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 5089

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl --fail http://localhost:5089/api/chat/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "Chatbot.API.dll"]
