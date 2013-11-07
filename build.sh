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

if ! hash gendarme 2>/dev/null; then
	echo "Gendarme is required to use the build script"	
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
if [ $? -eq 1 ]; then
	echo "Build failed"
	exit 1
fi

# Run unit tests
nunit-console2 StyleCopCmd.Core.Test/bin/$BUILD/StyleCopCmd.Core.Test.dll -noshadow
if [ $? -eq 1 ]; then
	echo "Unit tests failed"
	exit 1
fi

# Code analysis
gendarme StyleCopCmd.Console/bin/$BUILD/StyleCopCmd.Core.dll
if [ $? -eq 1 ]; then
	echo "Core project has a code analysis issue"
	exit 1
fi

gendarme StyleCopCmd.Console/bin/$BUILD/StyleCopCmd.Console.exe
if [ $? -eq 1 ]; then
	echo "Console project has a code analysis issue"
	exit 1
fi

# And check style
mono StyleCopCmd.Console/bin/$BUILD/StyleCopCmd.Console.exe -s StyleCopCmd.sln
if [ $? -eq 1 ]; then
	echo "StyleCop found a violation in the solution"
	exit 1
fi

# Update the readme
LINE=$(cat -n README.md | grep "``````text" | tail -1 | cut -f 1)
head -n $LINE README.md > README.tmp
mono StyleCopCmd.Console/bin/$BUILD/StyleCopCmd.Console.exe --help >> README.tmp
echo "\`\`\`" >> README.tmp
echo ""
echo "Updating readme if necessary"
diff -w README.md README.tmp
if [ $? -eq 1 ]; then
	mv README.tmp README.md
fi

echo "All done"
