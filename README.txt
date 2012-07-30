This is modified version of the original StyleCopCmd that used more recent versions of the dependencies and is mainly targeted at running in a Mono + Linux environment

Usage
------
$ mono StyleCopCmd.Console.exe -s <solution>
$ mono StyleCopCmd.Console.exe -f <file.cs>
$ mono StyleCopCmd.Console.exe -p <proj.csproj> -p <proj.csproj>

When building:
* Make sure to run StyleCopCmd over the project itself
(mono StyleCopCmd.Console.exe -s StyleCopCmd.sln)

Dependencies:
Run dll/build.sh to pull down the required dependencies
