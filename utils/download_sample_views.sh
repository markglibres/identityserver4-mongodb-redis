#!/usr/bin/env bash
set -e

BASE_PATH=https://raw.githubusercontent.com/markglibres/identityserver4-mongodb-redis/master/src/Server/Hosts/IdentityServer.Hosts.Mvc/

mkdir -p ./Views
touch ./Views/_ViewImports.cshtml
echo -e "@using IdentityServer.Hosts.Mvc.ViewModels" >> ./Views/_ViewImports.cshtml
echo -e "@using IdentityServer.HostServer.Mvc.ViewModels" >> ./Views/_ViewImports.cshtml
echo -e "@using IdentityServer.Hosts.Mvc.Controllers" >> ./Views/_ViewImports.cshtml

FILES=()
FILES+=("Views/Account/LoggedOut.cshtml")
FILES+=("Views/Account/Login.cshtml")
FILES+=("Views/Registration/ConfirmError.cshtml")
FILES+=("Views/Registration/CreateUser.cshtml")
FILES+=("Views/Registration/ForgotPassword.cshtml")
FILES+=("Views/Registration/ProfileUpdated.cshtml")
FILES+=("Views/Registration/ResetPassword.cshtml")
FILES+=("Views/Registration/ResetPasswordSent.cshtml")
FILES+=("Views/Registration/UpdatePassword.cshtml")
FILES+=("Views/Registration/UserCreated.cshtml")
FILES+=("Templates/user-email-confirmation.html")
FILES+=("Templates/user-resetpassword-link.html")

for file in "${FILES[@]}"
do
   curl "${BASE_PATH}${file}" --create-dirs --output "./${file}"
done
