#!/bin/bash

# Set environment variable
ENV=${1:-Production}

echo "Starting deployment with environment: $ENV"

# Stop and remove existing containers first
echo "Stopping existing containers..."
ENV="$ENV" docker-compose down --remove-orphans

# Remove dangling images from docker images AFTER stopping containers
echo "Cleaning up dangling images..."
docker images -f dangling=true -q | xargs -r docker rmi

# Build docker images using docker-compose (force rebuild)
echo "Building images..."
ENV="$ENV" docker-compose build

# Check if build was successful
if [ $? -ne 0 ]; then
    echo "Build failed! Stopping deployment."
    exit 1
fi

# Start containers using docker-compose
echo "Starting containers..."
ENV="$ENV" docker-compose up -d

# Wait a moment for containers to start
sleep 5

# Show running containers
echo "Running containers:"
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

# Check if any containers are running
RUNNING_CONTAINERS=$(docker ps -q | wc -l)
if [ $RUNNING_CONTAINERS -eq 0 ]; then
    echo "Warning: No containers are running!"
    echo "Checking container logs..."
    docker-compose logs
else
    echo "Successfully deployed $RUNNING_CONTAINERS container(s)"
fi

# Final cleanup of unused Docker resources
echo "Cleaning up unused Docker resources..."
docker system prune -f --volumes
docker image prune -f -a
docker container prune -f

echo "Deployment completed!"