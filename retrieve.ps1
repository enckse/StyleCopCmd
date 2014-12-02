<#
.SYNOPSIS
    Provides a simple powershell interface to setup dev for StyleCopCmd in Windows
.DESCRIPTION
    Downloads dependencies required to build/run/test StyleCopCmd
.EXAMPLE
    .\retrieve.ps1
#>

$msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe"
$binFolder = "$pwd\bin"
$stylecop = "sc"
$ndesk = "nd"
$nunit = "nu"
$gdrm = "gd"

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