#!/bin/bash
set -e

FILEPATH=$(dirname "$0")
SCRIPTDIR=$(cd "$(dirname "$FILEPATH")"; pwd -P)/$(basename "$FILEPATH")

NUPKGS_PATH=()

cd $SCRIPTDIR
cd ../../src/Server/IdentityServer
NUPKGS_PATH=(${NUPKGS_PATH[@]} $(pwd))
dotnet pack IdentityServer.csproj -c Release --output nupkgs
git config --local user.email "markglibres@gmail.com"
git config --local user.name "GitHub Action"
git add IdentityServer.csproj
git commit -m "bump version" -a

cd $SCRIPTDIR
cd ../../src/Server/Hosts/IdentityServer.Hosts.Server
NUPKGS_PATH=(${NUPKGS_PATH[@]} $(pwd))
dotnet pack IdentityServer.Hosts.Server.csproj -c Release --output nupkgs
git config --local user.email "markglibres@gmail.com"
git config --local user.name "GitHub Action"
git add IdentityServer.Hosts.Server.csproj
git commit -m "bump version" -a

cd $SCRIPTDIR
cd ../../src/Server/IdentityServer.User.Client
NUPKGS_PATH=(${NUPKGS_PATH[@]} $(pwd))
dotnet pack IdentityServer.User.Client.csproj -c Release --output nupkgs
git config --local user.email "markglibres@gmail.com"
git config --local user.name "GitHub Action"
git add IdentityServer.User.Client.csproj
git commit -m "bump version" -a

echo "nuget packages ${NUPKGS_PATH}"
for value in "${NUPKGS_PATH[@]}"
do
     dotnet nuget push $value/nupkgs/*.nupkg --api-key ${NUGET_APIKEY} --source https://api.nuget.org/v3/index.json
done
