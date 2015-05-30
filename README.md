Notice
------
(2015-05-30): StyleCop itself may or may not be 'dead' as a project BUT there is good news in that with 
all the recent Microsoft open source changes - it is really being reborn. I do not know what that means 
for StyleCopCmd or how it will interact going forward. I think that will require a more stable core of 
analyzers. I'll have to take a further look later as to whether StyleCopCmd even needs to continue to 
exist or not. For now, if there is a new release of the 'current' StyleCop, I'll update otherwise it is 
just a matter of what the future brings.

The new analyzing stuff for StyleCop is here: https://github.com/DotNetAnalyzers/StyleCopAnalyzers

General
-------

This is a (highly) modified version of the original StyleCopCmd that uses more recent versions of the dependencies and is mainly targeted at running in a Mono + Linux environment (but there is no known reason it shouldn't run elsewhere)

Installing
----------
* Download the latest release or build from source. 
* For Windows, run StyleCopCmd.Console.exe from the install location or add to the PATH
* For Linux, run mono StyleCopCmd.Console.exe from the install location (potentially create an alias to handle calling "mono StyleCopCmd" in an executable area)

Building
--------
[![Build Status](https://travis-ci.org/enckse/StyleCopCmd.png)](https://travis-ci.org/enckse/StyleCopCmd)
* Run make (Linux) or build.ps1 (Windows)
* The Windows script will (or should) pull all referenced items (gendarme, nunit, stylecop, ndesk)
* If scripts are added into the bin folder of the project, the powershell script needs to know NOT to delete those
* Make sure to run nunit-console with the noshadow option

Usage
------
Linux/Mono
```text
mono StyleCopCmd.Console.exe -s <solution>

mono StyleCopCmd.Console.exe -f <file.cs>

mono StyleCopCmd.Console.exe -p <proj.csproj> -p <proj.csproj>

mono StyleCopCmd.Console.exe -p <proj.csproj> -g=Console -l AllowCaching=false WriteCache=false
```

Known Issues
-----------
* Spell checking rules do not work in a Linux environment. This is due to StyleCop using office-based spell checking. (Check out LinuxCopExtensions for similar functionality in Linux)

Options
--------
```text
StyleCopCmd
Provides a command line interface for using StyleCop
  -s, --solutionFiles=VALUE  Solution files to check (.sln)
  -p, --projectFiles=VALUE   Project files to check (.csproj)
  -i, --ignoreFilePattern=VALUE
                             Regular expression patterns to ignore files
  -d, --directories=VALUE    Directories to check for files (.cs)
  -f, --files=VALUE          Files to check (.cs)
  -r, --recurse              Recursive directory search
  -c, --styleCopSettingsFile=VALUE
                             Use the given StyleCop settings file
  -o, --outputXmlFile=VALUE  The file the XML output is written to
  -x, --configurationSymbols=VALUE
                             Configuration symbols to pass to StyleCop (ex. 
                               DEBUG, RELEASE)
  -v, --violations           Print all violation information instead of the 
                               summary
  -q, --quiet                Do not print any information to console
  -?, --help                 Print the usage information
  -e, --eliminate            Eliminate checking duplicate files/projects
  -w, --withDebug            Perform checks with debug output
  -a, --addIns=VALUE         Addin paths to search
  -t, --terminate            Report a non-zero exit code on violation
  -u, --unloadProjects=VALUE Regular expressions to unload/ignore projects in 
                               a solution
  -m, --maxViolations=VALUE  Report a non-zero exit for any violations beyond 
                               this value
  -g, --generator=VALUE      Specify a generator engine for the analysis.
                                Console - the default generator with caching 
                               and full console reporting available.
                                Xml - A generator that only outputs the results 
                               file. Good for large projects.
  -l, --list                 Include a set of optional parameters (key=value 
                               or key:value). Known optional parameters 
                               include: AllowCaching, WriteCache
```
