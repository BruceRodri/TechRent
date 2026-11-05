#!/bin/sh
set -eu
dotnet tool install --tool-path /tmp/tools dotnet-sonarscanner >/dev/null
/tmp/tools/dotnet-sonarscanner begin /k:TechRentDistribuido /d:sonar.host.url=http://techrent-sonar-lab:9000 /d:sonar.token="$SONAR_TOKEN" /d:sonar.qualitygate.wait=true
dotnet build TechRent.csproj --no-incremental
/tmp/tools/dotnet-sonarscanner end /d:sonar.token="$SONAR_TOKEN"
