export MONGO_USERNAME="${MONGO_USERNAME:-admin}"
export MONGO_PASSWORD="${MONGO_PASSWORD:-admin}"
export MONGO_PORTS="${MONGO_PORTS:-27017:27017}"
export DB_VOLUME_PATH="${DB_VOLUME_PATH:-/data/db}"

export ASPNETCORE_ENVIRONMENT="${ASPNETCORE_ENVIRONMENT:-Development}"

export ELASTIC_PASSWORD="${ELASTIC_PASSWORD:-elasticdemo}"
export KIBANA_PASSWORD="${KIBANA_PASSWORD:-kibanademo}"
export DEVOPS_USER_PASSWORD="${DEVOPS_USER_PASSWORD:-devopsdemo}"


if [ "$1" = "production" ] ; then
    echo "Starting with production environment"
    docker compose up --build --force-recreate -d && docker container restart nginx

else
    echo "Starting with development environment"
    docker compose -f docker-compose-local.yml up --force-recreate --build -d

fi

docker rm -f set_elasticsearch_users
