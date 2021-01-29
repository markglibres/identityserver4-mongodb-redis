#!/usr/bin/env bash
set -e

BASE_PATH=https://raw.githubusercontent.com/markglibres/identityserver4-mongodb-redis/master/src/Clients/IdentityServer.Client.Mvc/

mkdir -p ./Views
touch ./Views/_ViewImports.cshtml
echo -e "\n@using IdentityServer.Client.Mvc" >> ./Views/_ViewImports.cshtml

FILES=()
FILES+=("Views/Home/Index.cshtml")
FILES+=("Views/Registration/New.cshtml")
FILES+=("Views/Shared/_Layout.cshtml")
FILES+=("Controllers/AccountController.cs")
FILES+=("Controllers/RegistrationController.cs")

for file in "${FILES[@]}"
do
   curl "${BASE_PATH}${file}" --create-dirs --output "./${file}"
done

