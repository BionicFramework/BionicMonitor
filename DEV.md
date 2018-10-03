# Development Notes

## Debug Build

```bash
dotnet build
```

## Building release from the command line:

```bash
dotnet build -c Release /p:SourceLinkCreate=true /p:VersionSuffix= /p:OfficialBuild=true
```

## Creating packages from command line:

```bash
dotnet pack -c Release /p:SourceLinkCreate=true /p:VersionSuffix= /p:OfficialBuild=true
```

## Add local package as nuget install sources

```bash
nuget sources add -name BionicMonitor -source $PWD/BionicMonitor/nupkg
nuget sources add -name BionicMonitorService -source $PWD/BionicMonitorService/nupkg
```

## Install or upgrade local Bionic Monitor version using latest package:

```bash
dotnet tool install -g BionicMonitor --add-source ./nupkg
```

```bash
dotnet tool update -g BionicMonitor --add-source ./nupkg
```

## Running the CLI
 
### Using the latest DLL
 
```bash
dotnet ./bin/Debug/netcoreapp2.1/BionicMonitor.dll -v
dotnet ./bin/Release/netcoreapp2.1/BionicMonitor.dll -v
```
 
### Using the CLI
 
```bash
biomon -v
```
