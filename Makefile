buildType=Release
all: clean download rebuild

rebuild: build test analyze

download:
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

test:
	nunit-console2 StyleCopCmd.Core.Test/bin/$(buildType)/StyleCopCmd.Core.Test.dll -noshadow

analyze:
	gendarme --ignore gendarme.ignore StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Core.dll
	gendarme --ignore gendarme.ignore StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe
	mono StyleCopCmd.Console/bin/$(buildType)/StyleCopCmd.Console.exe -s StyleCopCmd.sln -t

clean:
	rm -rf bin/*.dll
	rm -rf StyleCopCmd.Console/bin
	rm -rf StyleCopCmd.Core/bin
	rm -rf StyleCopCmd.Core.Test/bin

release:
	zip -j StyleCopCmd-maj.min.rev.zip StyleCopCmd.Console/bin/$(buildType)/*.dll StyleCopCmd.Console/bin/$(buildType)/*.exe