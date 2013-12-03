$gendarme = "gendarme.exe"
$nunit = "nunit-console.exe"
$msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe"
$binFolder = "$pwd\bin"
$oldREADME = "$pwd\README.md"
$newREADME = "$pwd\README.tmp"
$stylecop = "sc"
$ndesk = "nd"
$buildType = "DEBUG"

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

if ($args -AND $args.Length -gt 0)
{
	if ($args[0].ToUpper() -eq "RELEASE")
	{
		$buildType = "RELEASE"
	}
}

GetAndUnzip "https://nuget.org/api/v2/package/StyleCop.MSBuild" $stylecop
mv "$binFolder\$stylecop\tools\StyleCop.dll" $binFolder -Force
mv "$binFolder\$stylecop\tools\StyleCop.CSharp.dll" $binFolder -Force
mv "$binFolder\$stylecop\tools\StyleCop.CSharp.Rules.dll" $binFolder -Force
GetAndUnzip "https://nuget.org/api/v2/package/NDesk.Options" $ndesk
mv "$binFolder\$ndesk\lib\NDesk.Options.dll" $binFolder -Force

# Requires nunit to either be in the bin folder or in the GAC or somewhere in the path for msbuild
# Maybe handle this later on and pull it down
& $msbuild /property:Configuration=$buildType StyleCopCmd.sln
if (-not $?)
{
	echo "Build failed"
	exit 1
}

& $nunit StyleCopCmd.Core.Test/bin/$buildType/StyleCopCmd.Core.Test.dll -noshadow
if (-not $?)
{
	echo "Unit tests failed"
	exit 1
}

& $gendarme --ignore gendarme.ignore StyleCopCmd.Console/bin/$buildType/StyleCopCmd.Core.dll
if (-not $?)
{
	echo "Core project has a code analysis issue"
	exit 1
}

& $gendarme --ignore gendarme.ignore StyleCopCmd.Console/bin/$buildType/StyleCopCmd.Console.dll
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
