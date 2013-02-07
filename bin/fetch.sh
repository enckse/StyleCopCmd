#!/bin/sh

# Pull stylecop from the nuget gallery
# Use NuGet in the future (currently it doesn't work properly on Linux + Mono)
mkdir sc-temp
cd sc-temp
wget -O sc.zip https://nuget.org/api/v2/package/StyleCop.MSBuild
unzip sc.zip
mv tools/StyleCop.dll ..
mv tools/StyleCop.CSharp.dll ..
mv tools/StyleCop.CSharp.Rules.dll ..
cd ..
rm -rf sc-temp

# Pull NDesk.Options too for command line parsing
mkdir nd-temp
cd nd-temp
wget -O ndesk.zip https://nuget.org/api/v2/package/NDesk.Options
unzip ndesk.zip
mv lib/NDesk.Options.dll ..
cd ..
rm -rf nd-temp
