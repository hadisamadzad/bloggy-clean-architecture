#!/bin/bash

# Remove none images from docker images BEFORE deploy
echo "Cleaning up dangling images..."
docker images -f dangling=true -q

# Build docker image using docker-compose
echo "Building images..."
sudo ENV="{Env}" docker-compose build

# Up docker image using docker-compose
echo "Starting containers..."
sudo ENV="{Env}" docker-compose -f docker-compose.yml up -d

# Remove unused images and containers from docker images AFTER deploy
echo "Final cleanup..."
docker system prune -a -f

echo "Deployment completed!"