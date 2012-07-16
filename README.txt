Changes from the base Mono StyleCopCmd:
* Adjust references to use a more recent version of StyleCop and adjust the source to compile using that version.
* "-of" is no longer a required argument (aka direct violation xml reports are now available without the transform)

-----------------------------------------------

This is stylecopcmd, the trunk as of 2010-08-20.

This version has been modified very slightly to build and run under mono on linux, 

Running:
--------

$ mono Net.SF.StyleCopCmd.Console.exe -sf <solution> -of /home/me/stylecop-reports/report.xml
$ mono Net.SF.StyleCopCmd.Console.exe -f <file.cs>   -of /home/me/stylecop-reports/report.xml

Known Issues:
-------------
You may also see 'permission denied' errors writing your report file, be sure that you specify 
a path where you can write to it's parent directory ( eg, -of /home/inb/stylecop-reports/report.xml )

So far, all the unit tests pass except for NAnt project creation.

The very first time you run the console tool you might see:-
 "An error occurred during the analysis: No access to the given key"

If so, you will have to invoke the command (only once) as root (sudo), 
stylecop wants to create a registry key, thats all.

Ian Norton

--
Please see the following URL for information regarding this project:

http://stylecopcmd.sourceforge.net/
