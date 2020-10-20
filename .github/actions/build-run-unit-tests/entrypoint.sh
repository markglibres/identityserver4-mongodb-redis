#!/bin/sh
set -e

PROJ="${INPUT_PROJECT}"
CONFIGURATION="${INPUT_CONFIGURATION}"

echo "work directory: ${INPUT_WORKDIR}"
[ ! -z "${INPUT_WORKDIR}" ] && cd "${INPUT_WORKDIR}"

echo "projects to run: ${PROJ}"

for proj in $PROJ; do
    echo "running project: ${proj}"
    dotnet restore -v q $proj
    dotnet build -v q $proj --configuration $CONFIGURATION
done

for proj in $PROJ; do
    dotnet test $proj --configuration $CONFIGURATION -l "console;verbosity=detailed" -v n --no-build
done