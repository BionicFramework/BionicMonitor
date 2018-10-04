#!/usr/bin/env bash

source ~/.nuget_bionic_key

if [ -z "$1" ]
  then
    echo "No version supplied"
    exit 1
fi

if [ -z "${NUGET_BIONIC_KEY}" ]
  then
    echo "No NuGet key found for Bionic"
    exit 1
fi

dotnet pack -c Release /p:SourceLinkCreate=true /p:VersionSuffix= /p:OfficialBuild=true

echo "Pushing BionicMonitor.$1.nupkg to NuGet..."
dotnet nuget push ./BionicMonitor/nupkg/BionicMonitor.$1.nupkg --source https://api.nuget.org/v3/index.json --api-key ${NUGET_BIONIC_KEY}
if [ $? = 0 ]
  then
    echo "Completed pushing BionicMonitor.$1.nupkg to NuGet. Should be available in a few minutes."
fi

echo "Pushing BionicMonitor.$1.nupkg to NuGet..."
dotnet nuget push ./BionicMonitorService/nupkg/BionicMonitorService.$1.nupkg --source https://api.nuget.org/v3/index.json --api-key ${NUGET_BIONIC_KEY}
if [ $? = 0 ]
  then
    echo "Completed pushing BionicMonitorService.$1.nupkg to NuGet. Should be available in a few minutes."
fi
