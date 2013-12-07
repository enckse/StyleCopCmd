<#
.SYNOPSIS
    Provides a simple powershell interface to build StyleCopCmd in Windows
.DESCRIPTION
    Downloads dependencies, builds the solution, runs the unit tests, code analysis, check for violations.
.PARAMETER release
    Build the project in RELEASE mode (instead of DEBUG)
.PARAMETER skipDownload
    Skip downloading dependencies
.EXAMPLE
    .\build.ps1 -release -skipDownload
    Build the project in RELEASE mode and do not download any dependencies
#>
param(
    [switch]$release,
    [switch]$skipDownload
    )

$msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe"
$binFolder = "$pwd\bin"
$oldREADME = "$pwd\README.md"
$newREADME = "$pwd\README.tmp"
$stylecop = "sc"
$ndesk = "nd"
$nunit = "nu"
$gdrm = "gd"
$buildType = "DEBUG"
$nunitExe = "$binFolder\nunit-console.exe"
$gendarmeExe = "$binFolder\gendarme.exe"

function GetAndUnzip($url, $file)
{
	$client = new-object System.Net.WebClient
	$folder = "$binFolder\$file"
	if (Test-Path $folder)
	{
		Remove-Item $folder -recurse
	}
	
	mkdir $folder | Out-Null
	$zipFile = "$folder\$file.zip"
	$client.DownloadFile($url, $zipFile)
	$shell = new-object -com shell.application
	$zip = $shell.NameSpace($zipFile)
	foreach($item in $zip.items())
	{
		$shell.Namespace($folder).copyhere($item)
	}
}

if ($release)
{
    $buildType = "RELEASE"
}

if (-not $skipDownload)
{
    # Avoid deleting the other scripts in this directory (for linux build)
    rm "$binFolder\*" -exclude *.LICENSE,*.sh,*.bat -recurse

    # Stylecop itself
    GetAndUnzip "https://nuget.org/api/v2/package/StyleCop.MSBuild" $stylecop
    mv "$binFolder\$stylecop\tools\StyleCop.dll" $binFolder -Force
    mv "$binFolder\$stylecop\tools\StyleCop.CSharp.dll" $binFolder -Force
    mv "$binFolder\$stylecop\tools\StyleCop.CSharp.Rules.dll" $binFolder -Force

    # ndesk
    GetAndUnzip "https://nuget.org/api/v2/package/NDesk.Options" $ndesk
    mv "$binFolder\$ndesk\lib\NDesk.Options.dll" $binFolder -Force

    # nunit
    GetAndUnzip "http://launchpad.net/nunitv2/trunk/2.6.3/+download/NUnit-2.6.3.zip" $nunit
    mv "$binFolder\$nunit\NUnit-2.6.3\bin\*" $binFolder -Force

    # gendarme
    GetAndUnzip "https://github.com/downloads/spouliot/gendarme/gendarme-2.10-bin.zip" $gdrm
    mv "$binFolder\$gdrm\*" $binFolder -Force
}

& $msbuild /property:Configuration=$buildType StyleCopCmd.sln
if (-not $?)
{
	echo "Build failed"
	exit 1
}

& $nunitExe StyleCopCmd.Core.Test/bin/$buildType/StyleCopCmd.Core.Test.dll -noshadow
if (-not $?)
{
	echo "Unit tests failed"
	exit 1
}

& $gendarmeExe --ignore gendarme.ignore StyleCopCmd.Console/bin/$buildType/StyleCopCmd.Core.dll
if (-not $?)
{
	echo "Core project has a code analysis issue"
	exit 1
}

& $gendarmeExe --ignore gendarme.ignore StyleCopCmd.Console/bin/$buildType/StyleCopCmd.Console.exe
if (-not $?)
{
	echo "Console project has a code analysis issue"
	exit 1
}

& "StyleCopCmd.Console/bin/$buildType/StyleCopCmd.Console.exe" -s StyleCopCmd.sln
if (-not $?)
{
	echo "StyleCop found a violation in the solution"
	exit 1
}

$lines = [System.IO.File]::ReadAllText($oldREADME)
$lastIndex = $lines.LastIndexOf("``````text")
echo $lines.substring(0, $lastIndex + 7) > $newREADME
& "StyleCopCmd.Console/bin/$buildType/StyleCopCmd.Console.exe" -h >> $newREADME
echo "``````" >> $newREADME
$compared = compare-object $oldREADME $newREADME
echo "Updating readme if necessary"
if ($compared.Count -gt 0)
{
	mv $newREADME $oldREADME -Force
}

echo "Done"
