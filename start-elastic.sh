# Set environment variable
export ELASTIC_PASSWORD="elasticdemo"
export KIBANA_PASSWORD="kibanademo"
export DEVOPS_USER_PASSWORD="devops1234"
docker compose -f docker-compose-elastic.yml up --build --force-recreate -d
docker rm -f set_elasticsearch_users