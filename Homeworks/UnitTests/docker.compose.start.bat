docker network create --driver=bridge --subnet=172.77.0.0/16 myLocalNetwork
SET COMPOSE_CONVERT_WINDOWS_PATHS=1

#docker-compose -f compose.yml build
docker-compose -f compose.yml up

pause