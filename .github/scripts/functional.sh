#!/bin/bash
set -e


FILEPATH=$(dirname "$0")
SCRIPTDIR=$(cd "$(dirname "$FILEPATH")"; pwd -P)/$(basename "$FILEPATH")

cd $SCRIPTDIR
cd ../../src

while test $# -gt 0
do
    case "$1"
    in
        -c|--collection)
            echo "collection"
            shift
            if test $# -gt 0
            then
                echo "collection supplied $1"
                COLLECTION=$1
            fi
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

docker_file='-f docker-compose.yaml'

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

if [[ -z $COLLECTION ]];
then
    exit 0;
fi

while [[ "$(curl -s -o /dev/null -w ''%{http_code}'' localhost:5000/.well-known/openid-configuration)" != "200" ]]
do 
    echo 'waiting for api...'
    sleep 5
done 

cd $SCRIPTDIR
cd ../../postman

ENVPATH=$(find . -name "functional.postman_environment.json" -print -quit)

find . -name $COLLECTION -print0 | 
    while IFS= read -r -d '' collection; do 
        echo "running project: ${collection}"
        newman run $collection -e "${ENVPATH}"
    done
