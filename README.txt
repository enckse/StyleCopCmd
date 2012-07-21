Changes from the base Mono StyleCopCmd:
* Adjust references to use a more recent version of StyleCop and adjust the source to compile using that version.
* "-of" is no longer a required argument (aka direct violation xml reports are now available without the transform)

Usage
------
$ mono Net.SF.StyleCopCmd.Console.exe -s <solution> -o /home/me/stylecop-reports/report.xml
$ mono Net.SF.StyleCopCmd.Console.exe -f <file.cs>   -o /home/me/stylecop-reports/report.xml
$ mono Net.SF.StyleCopCmd.Console.exe -p <proj.csproj> -p <proj.csproj>

When building:
* Make sure to run StyleCopCmd over the project itself, exclude the test project:
(mono Net.SF.StyleCopCmd.Console.exe -p src/Net.SF.StyleCopCmd.Core/Net.SF.StyleCopCmd.Core.csproj -p src/Net.SF.StyleCopCmd.Console/Net.SF.StyleCopCmd.Console.csproj)

Dependencies:
Run dll/build.sh to pull down the required dependencies
