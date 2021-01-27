#!/bin/bash
set -e

FILEPATH=$(dirname "$0")
SCRIPTDIR=$(cd "$(dirname "$FILEPATH")"; pwd -P)/$(basename "$FILEPATH")

cd $SCRIPTDIR
cd ../src

while test $# -gt 0
do
    case "$1"
    in
        --run)
            echo "run app"
            RUN=true
            shift
            ;;
        --up)
            echo "run services"
            UP=true
            shift
            ;;
        --down)
            echo "stop services"
            DOWN=true
            shift
            ;;
        *)
            shift
            break;;
    esac
done

docker_file='-f docker-compose-db.yaml'

if [[ $DOWN == true ]]
then
    docker-compose $docker_file down
    exit 0
fi

if [[ $UP == true ]]
then
    docker-compose $docker_file build
    docker-compose $docker_file up -d --remove-orphans
fi

if [[ -z $RUN ]];
then
    exit 0;
fi