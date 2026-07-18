export ASPNETCORE_ENVIRONMENT="${ASPNETCORE_ENVIRONMENT:-Production}"

docker compose up --force-recreate --build -d && docker ps
