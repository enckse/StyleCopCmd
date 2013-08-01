#!/bin/bash
if ! hash xbuild 2>/dev/null; then
	echo "xbuild is required to use the build script"	
	exit 1
fi

if ! hash nunit-console2 2>/dev/null; then
	echo "nunit (nunit-console2) is required to use the build script"	
	exit 1
fi

if ! hash mono 2>/dev/null; then
	echo "mono is required to use the build script"	
	exit 1
fi

# Build type
BUILD="$(tr [A-Z] [a-z] <<< "$1")"
if [ "$BUILD" = "release" ]
then
      BUILD="Release"
else
      BUILD="Debug"
fi

# Pull resources
cd bin
./fetch.sh

# Build
cd ..
xbuild /property:Configuration=$BUILD StyleCopCmd.sln

# Run unit tests
nunit-console2 StyleCopCmd.Core.Test/bin/$BUILD/StyleCopCmd.Core.Test.dll -noshadow

# And check style
mono StyleCopCmd.Console/bin/$BUILD/StyleCopCmd.Console.exe -s StyleCopCmd.sln
