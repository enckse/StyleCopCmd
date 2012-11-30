#!/bin/bash

# Build type
BUILD="$(tr [A-Z] [a-z] <<< "$1")"
if [ "$BUILD" = "release" ]
then
      BUILD="Release"
else
      BUILD="Debug"
fi

# Pull resources
cd dll
./fetch.sh

# Build
cd ..
xbuild /property:Configuration=$BUILD StyleCopCmd.sln

# Run unit tests
nunit-console2 StyleCopCmd.Core.Test/bin/Debug/StyleCopCmd.Core.Test.dll -noshadow

# And check style
mono StyleCopCmd.Console/bin/$BUILD/StyleCopCmd.Console.exe -s StyleCopCmd.sln
