# Docker Deployment Guide

This guide explains how to build and run the Chatbot API using Docker.

## Prerequisites

- Docker (20.10+)
- Docker Compose (1.29+)

## Quick Start

### 1. Build and Run with Docker Compose

The easiest way to get started is using Docker Compose:

```bash
# Build and start the containers
docker-compose up -d

# View logs
docker-compose logs -f chatbot-api

# Stop the containers
docker-compose down

# Stop and remove volumes (fresh start)
docker-compose down -v
```

The API will be available at `http://localhost:5089`

### 2. Build and Run with Docker Commands

Alternatively, you can use Docker commands directly:

```bash
# Build the image
docker build -t chatbot-api:latest .

# Run the container
docker run -d \
  --name chatbot-api \
  -p 5089:5089 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e "Jwt__Key=your-secret-key-here" \
  -v chatbot-data:/app/data \
  chatbot-api:latest

# View logs
docker logs -f chatbot-api

# Stop and remove the container
docker stop chatbot-api
docker rm chatbot-api
```

## Configuration

### Environment Variables

You can configure the application using environment variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | Production |
| `ASPNETCORE_URLS` | Listening URLs | http://+:5089 |
| `ConnectionStrings__DefaultConnection` | Database connection string | Data Source=/app/data/chatbot.db |
| `Jwt__Key` | JWT signing key (MUST change in production) | - |
| `Jwt__Issuer` | JWT token issuer | ChatbotAPI |
| `Jwt__Audience` | JWT token audience | ChatbotClient |
| `Jwt__ExpireMinutes` | JWT token expiration time (minutes) | 1440 |

### Using .env File

Create a `.env` file in the same directory as `docker-compose.yml`:

```env
JWT_SECRET_KEY=your-very-secure-secret-key-minimum-32-characters-long
ASPNETCORE_ENVIRONMENT=Production
```

Then run:

```bash
docker-compose --env-file .env up -d
```

## Health Checks

The container includes a health check that monitors the API status:

```bash
# Check container health status
docker ps

# View health check logs
docker inspect chatbot-api | grep -A 10 Health
```

## Data Persistence

The application uses a SQLite database that is stored in a Docker volume:

```bash
# List volumes
docker volume ls

# Inspect the data volume
docker volume inspect chatbot_chatbot-data

# Backup the database
docker cp chatbot-api:/app/data/chatbot.db ./backup/

# Restore the database
docker cp ./backup/chatbot.db chatbot-api:/app/data/
```

## Production Deployment

### Security Considerations

1. **Change the JWT Secret Key**: Always use a strong, unique secret key in production
2. **Use HTTPS**: Set up a reverse proxy (nginx/traefik) with SSL/TLS certificates
3. **Database**: Consider using a more robust database (PostgreSQL/SQL Server) for production
4. **Secrets Management**: Use Docker secrets or environment variable injection from a secure vault

### Example with HTTPS (nginx)

1. Uncomment the nginx service in `docker-compose.yml`
2. Create an `nginx.conf` file:

```nginx
events {
    worker_connections 1024;
}

http {
    upstream chatbot_api {
        server chatbot-api:5089;
    }

    server {
        listen 80;
        server_name yourdomain.com;
        
        location / {
            return 301 https://$server_name$request_uri;
        }
    }

    server {
        listen 443 ssl http2;
        server_name yourdomain.com;

        ssl_certificate /etc/nginx/ssl/cert.pem;
        ssl_certificate_key /etc/nginx/ssl/key.pem;

        location / {
            proxy_pass http://chatbot_api;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection "upgrade";
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }
    }
}
```

3. Run with nginx:

```bash
docker-compose up -d
```

## Scaling

To run multiple instances behind a load balancer:

```bash
# Scale the API service to 3 instances
docker-compose up -d --scale chatbot-api=3

# Use nginx or another load balancer to distribute traffic
```

**Note**: When scaling, consider:
- Using Redis for distributed rate limiting (currently in-memory)
- Shared database (not SQLite)
- Session affinity for WebSocket connections

## Troubleshooting

### Container won't start

```bash
# Check logs
docker-compose logs chatbot-api

# Check if port is already in use
sudo lsof -i :5089

# Rebuild the image
docker-compose build --no-cache
docker-compose up -d
```

### Database issues

```bash
# Reset the database (WARNING: deletes all data)
docker-compose down -v
docker-compose up -d

# Access the database directly
docker exec -it chatbot-api sqlite3 /app/data/chatbot.db
```

### Health check failing

```bash
# Test the health endpoint manually
docker exec chatbot-api curl http://localhost:5089/api/chat/health

# Check if the application is running
docker exec chatbot-api ps aux
```

## Monitoring

### View real-time logs

```bash
# All logs
docker-compose logs -f

# API logs only
docker-compose logs -f chatbot-api

# Last 100 lines
docker-compose logs --tail=100 chatbot-api
```

### Resource usage

```bash
# View resource usage
docker stats chatbot-api

# View detailed container info
docker inspect chatbot-api
```

## Cleanup

```bash
# Stop and remove containers
docker-compose down

# Remove containers and volumes (deletes data)
docker-compose down -v

# Remove the image
docker rmi chatbot-api:latest

# Clean up all unused Docker resources
docker system prune -a
```

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Build and Deploy

on:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Build Docker image
        run: docker build -t chatbot-api:${{ github.sha }} .
      
      - name: Run tests
        run: docker run chatbot-api:${{ github.sha }} dotnet test
      
      - name: Push to registry
        run: |
          echo "${{ secrets.DOCKER_PASSWORD }}" | docker login -u "${{ secrets.DOCKER_USERNAME }}" --password-stdin
          docker push chatbot-api:${{ github.sha }}
```

## Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [ASP.NET Core Docker Documentation](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)
