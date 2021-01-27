#!/bin/bash
set -e

FILEPATH=$(dirname "$0")
SCRIPTDIR=$(cd "$(dirname "$FILEPATH")"; pwd -P)/$(basename "$FILEPATH")

VERSION=""
DIRECTORY=""
PROJECT=""
while test $# -gt 0
do
    case "$1"
    in
        -v|--version)
            echo "version"
            shift
            if test $# -gt 0
            then
                echo "version supplied $1"
                VERSION=$1
            fi
            shift
            ;;
        -d|--dir)
            echo "directory"
            shift
            if test $# -gt 0
            then
                echo "directory supplied $1"
                DIRECTORY=$1
            fi
            shift
            ;;
        -p|--proj)
            echo "project"
            shift
            if test $# -gt 0
            then
                echo "project supplied $1"
                PROJECT=$1
            fi
            shift
            ;;
        *)
            shift
            break;;
    esac
done

cd $DIRECTORY
dotnet pack $PROJECT -c Release --output nupkgs -p:PackageVersion=$VERSION
dotnet nuget push nupkgs/*.nupkg --api-key ${NUGET_APIKEY} --source https://api.nuget.org/v3/index.json
