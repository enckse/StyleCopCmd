Changes from the base Mono StyleCopCmd:
* Adjust references to use a more recent version of StyleCop and adjust the source to compile using that version.
* "-of" is no longer a required argument (aka direct violation xml reports are now available without the transform)

Usage
------
$ mono Net.SF.StyleCopCmd.Console.exe -sf <solution> -of /home/me/stylecop-reports/report.xml
$ mono Net.SF.StyleCopCmd.Console.exe -f <file.cs>   -of /home/me/stylecop-reports/report.xml
$ mono Net.SF.StyleCopCmd.Console.exe -pf <proj.csproj> <proj.csproj>

Forked to create a more updated version
Please see the previous version for a few additional notes/known issues:
https://github.com/inorton/StyleCopCmd
