This is modified version of the original StyleCopCmd that used more recent versions of the dependencies and is mainly targeted at running in a Mono + Linux environment

Installing
----------
* Build from source or download the latest posted build in Downloads. 
* For Windows, run StyleCopCmd.Console.exe from the install location or add to the PATH
* For Linux, run mono StyleCopCmd.Console.exe from the install location (potentially create an alias to handle calling "mono StyleCopCmd" in an executable area)

Usage
------
```text
mono StyleCopCmd.Console.exe -s <solution>

mono StyleCopCmd.Console.exe -f <file.cs>

mono StyleCopCmd.Console.exe -p <proj.csproj> -p <proj.csproj>
```

When building:
* Run dll/build.sh to pull down the required dependencies
* Make sure to run StyleCopCmd over the project itself
(mono StyleCopCmd.Console.exe -s StyleCopCmd.sln)

Options
--------
```text
StyleCopCmd
Provides an interface for using StyleCop (specifically for Mono on Linux)
  -s, --solutionFiles=VALUE  Visual studio solution files to check
  -p, --projectFiles=VALUE   Visual Studio project files to check
  -i, --ignoreFilePattern=VALUE
                             Regular expression patterns to ignore files
  -d, --directories=VALUE    Directories to check for CSharp files
  -f, --files=VALUE          Files to check
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
```

Known Issues
-----------
* Spell checking rules do not work in a Linux environment. This is due to StyleCop using office-based spell checking. (Check out LinuxCopExtensions for similar functionality in Linux)
