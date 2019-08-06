#!/usr/bin/env bash -e

echo “Found Unit test projects:”

find $APPCENTER_SOURCE_DIRECTORY -regex ‘*Tests\*.csproj’ -exec echo {} \;

echo

echo “Run Unit test projects:”

find $APPCENTER_SOURCE_DIRECTORY -regex ‘*Tests\*.csproj’ | xargs dotnet test;