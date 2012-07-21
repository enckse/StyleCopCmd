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

# Pull the nant release as well
mkdir nant-temp
cd nant-temp
wget -O nant.zip http://sourceforge.net/projects/nant/files/nant/0.92/nant-0.92-bin.zip/download
unzip nant.zip
mv nant-0.92/bin/NAnt.Core.dll ..
mv nant-0.92/bin/log4net.dll ..
cd ..
rm -rf nant-temp
