# Remove none images from docker images BEFORE deploy
docker images -f dangling=true -q

# Build docker image from dockerfile

# Build docker image using docker-compose
sudo ENV="{Env}" docker-compose build

# Up docker image using docker-compose
sudo ENV="{Env}" docker-compose -f docker-compose.yml up -d

# Remove unused images and containers from docker images AFTER deploy
docker system prune -a -f