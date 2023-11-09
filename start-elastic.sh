# Set environment variable
export MONGO_PORTS="${MONGO_PORTS:-27017:27017}"
export ELASTIC_PASSWORD="${ELASTIC_PASSWORD:-elasticdemo}"
export KIBANA_PASSWORD="${KIBANA_PASSWORD:-kibanademo}"
export DEVOPS_USER_PASSWORD="${DEVOPS_USER_PASSWORD:-devopsdemo}"
docker compose -f docker-compose-elastic.yml up --build --force-recreate -d
# docker rm -f set_elasticsearch_users