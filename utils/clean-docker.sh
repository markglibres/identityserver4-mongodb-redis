docker stop $(docker ps -a -q)

docker system prune -f
docker volume prune -f