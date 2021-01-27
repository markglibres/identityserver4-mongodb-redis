#!/usr/bin/env bash
set -e

BASE_PATH=https://raw.githubusercontent.com/markglibres/identityserver4-mongodb-redis/master/src/Server/Hosts/IdentityServer.Hosts.Mvc/

FILES=()
FILES+=("Views/_ViewStart.cshtml")
FILES+=("Views/_ViewImports.cshtml")
FILES+=("Views/Account/LoggedOut.cshtml")
FILES+=("Views/Account/Login.cshtml")
FILES+=("Views/Home/Error.cshtml")
FILES+=("Views/Registration/ConfirmError.cshtml")
FILES+=("Views/Registration/CreateUser.cshtml")
FILES+=("Views/Registration/ForgotPassword.cshtml")
FILES+=("Views/Registration/ProfileUpdated.cshtml")
FILES+=("Views/Registration/ResetPassword.cshtml")
FILES+=("Views/Registration/ResetPasswordSent.cshtml")
FILES+=("Views/Registration/UpdatePassword.cshtml")
FILES+=("Views/Registration/UserCreated.cshtml")

for file in "${FILES[@]}"
do
   curl "${BASE_PATH}${file}"  --create-dirs --output ${file}
done
