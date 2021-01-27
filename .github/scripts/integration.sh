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
        --run)
            echo "run tests"
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

docker_file='-f docker-compose-ci_integration.yaml'

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

while [[ "$(curl -k -s -o /dev/null -w ''%{http_code}'' https://localhost:2113/stats -u 'admin:changeit')" != "200" ]]
do 
    echo 'waiting for eventstore...'
    sleep 5
done 

find . -name "*Integration*Test*.csproj" -print0 | 
    while IFS= read -r -d '' proj; do 
        echo "running project: ${proj}"
        dotnet restore -v q $proj
        dotnet build -v q $proj
        dotnet test  -l "console;verbosity=detailed" -v n $proj --no-build 
    done
