export MONGO_USERNAME="${MONGO_USERNAME:-admin}"
export MONGO_PASSWORD="${MONGO_PASSWORD:-admin}"
export MONGO_PORTS="${MONGO_PORTS:-27017:27017}"

export ASPNETCORE_ENVIRONMENT="${ASPNETCORE_ENVIRONMENT:-Development}"

docker compose up --force-recreate --build -d && docker container restart nginx
