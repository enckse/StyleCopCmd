<#
.SYNOPSIS
    Provides a simple powershell interface to build StyleCopCmd in Windows
.DESCRIPTION
    Downloads dependencies, builds the solution, runs the unit tests, code analysis, check for violations.
.PARAMETER release
    Build the project in RELEASE mode (instead of DEBUG)
.PARAMETER skipDownload
    Skip downloading dependencies
.PARAMETER skipBuild
    Skip building
.PARAMETER skipTest
    Skip testing
.PARAMETER skipAnalyze
    Skip the analysis steps
.EXAMPLE
    .\build.ps1 -release -skipDownload
    Build the project in RELEASE mode and do not download any dependencies
#>
param(
    [switch]$release,
    [switch]$skipDownload,
    [switch]$skipBuild,
    [switch]$skipTest,
    [switch]$skipAnalyze
    )

if ($skipDownload -AND $skipBuild -AND $skipTest -AND $skipAnalyze)
{
    Write-Host -foregroundcolor "RED" "Invalid build settings";
    exit
}
    
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
    rm "$binFolder\*" -exclude *.LICENSE -recurse

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

if (-not $skipBuild)
{
    & $msbuild /property:Configuration=$buildType StyleCopCmd.sln
    if (-not $?)
    {
        echo "Build failed"
        exit 1
    }
}

if (-not $skipTest)
{
    & $nunitExe StyleCopCmd.Core.Test/bin/$buildType/StyleCopCmd.Core.Test.dll -noshadow
    if (-not $?)
    {
        echo "Unit tests failed"
        exit 1
    }
}

if (-not $skipAnalyze)
{
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
}
