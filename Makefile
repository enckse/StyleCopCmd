buildType=Release
version:= `cat StyleCopCmd.Core/Properties/CommonAssemblyInfo.cs | grep "AssemblyFileVersion"| cut -f2 -d '"' | cut -f1,2,3 -d '.' | awk '{print $0}'`

all: download build

ci: download integration

release: ci package

clean:
	rm -rf bin/*.dll
	rm -rf StyleCopCmd.Console/bin
	rm -rf StyleCopCmd.Core/bin
	rm -rf StyleCopCmd.Core.Test/bin

download: clean
	mkdir -p bin/tmp
	rm -rf bin/tmp/*
	wget -O bin/tmp/sc.zip https://nuget.org/api/v2/package/StyleCop.MSBuild
	unzip bin/tmp/sc.zip -d bin/tmp
	mv bin/tmp/tools/StyleCop.dll bin/
	mv bin/tmp/tools/StyleCop.CSharp.dll bin/
	mv bin/tmp/tools/StyleCop.CSharp.Rules.dll bin/ 
	rm -r bin/tmp/*
	wget -O bin/tmp/ndesk.zip https://nuget.org/api/v2/package/NDesk.Options
	unzip bin/tmp/ndesk.zip -d bin/tmp
	mv bin/tmp/lib/NDesk.Options.dll bin/

build:
	xbuild /property:Configuration="$(buildType)" StyleCopCmd.sln

test: build
	nunit-console2 StyleCopCmd.Core.Test/bin/$(buildType)/StyleCopCmd.Core.Test.dll -noshadow

analyze: test
	gendarme --ignore gendarme.ignore StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Core.dll
	gendarme --ignore gendarme.ignore StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -s StyleCopCmd.sln -t

integration: analyze
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -s StyleCopCmd.sln -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -s StyleCopCmd.sln -v -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -s StyleCopCmd.sln -g=Xml -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -d . -r -i ClassOne.cs -i ClassTwo.cs -i AssemblyInfo.cs -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -d . -r -i ClassOne.cs -i ClassTwo.cs -i AssemblyInfo.cs -v -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -d . -r -i ClassOne.cs -i ClassTwo.cs -i AssemblyInfo.cs -g=Xml -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -p StyleCopCmd.Core/StyleCopCmd.Core.csproj -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -p StyleCopCmd.Core/StyleCopCmd.Core.csproj -v -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -p StyleCopCmd.Core/StyleCopCmd.Core.csproj -g=Xml -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -p StyleCopCmd.Core.Test/StyleCopCmd.Core.Test.csproj -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -p StyleCopCmd.Core.Test/StyleCopCmd.Core.Test.csproj -v -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -p StyleCopCmd.Core.Test/StyleCopCmd.Core.Test.csproj -g=Xml -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -p StyleCopCmd.Console/StyleCopCmd.Console.csproj -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -p StyleCopCmd.Console/StyleCopCmd.Console.csproj -v -t
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -p StyleCopCmd.Console/StyleCopCmd.Console.csproj -g=Xml -t

package:
	zip -j StyleCopCmd-$(version).zip StyleCopCmd.Console/bin/$(buildType)/*.dll StyleCopCmd.Console/bin/$(buildType)/*.exe
