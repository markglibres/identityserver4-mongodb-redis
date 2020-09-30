#!/bin/bash
set -e

FILEPATH=$(dirname "$0")
SCRIPTDIR=$(cd "$(dirname "$FILEPATH")"; pwd -P)/$(basename "$FILEPATH")

cd $SCRIPTDIR
cd ../../src/IdentityServer

dotnet pack IdentityServer.csproj -c Release --output nupkgs
dotnet nuget push nupkgs/*.nupkg --api-key ${NUGET_APIKEY} --source https://api.nuget.org/v3/index.json