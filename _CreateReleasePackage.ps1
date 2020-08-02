param ($config)

# Get the version found in the project
$infoPath = "D:\Users\Lawrence\Documents\Projects\GW2BuildLibrary\Properties\AssemblyInfo.cs"
$version = Select-String -Path $infoPath -Pattern '^\[assembly: AssemblyVersion\("(.*)"\)\]$'  | % {"$($_.matches.groups[1])"}

$arr = $version.Split(".")
$tag = "INVALID"
switch ($arr[3])
{
	"0" { $tag = "" }
	"1" { $tag = "-Pre" }
	"2" { $tag = "-Beta" }
	"3" { $tag = "-Alpha" }
}

if ($tag -eq "INVALID") {
	write-host -f red "Invalid version: '$version'"
} else {
	$major = $arr[0]
	$minor = $arr[1]
	$patch = $arr[2]
	
	$version = "v$major.$minor.$patch$tag"

	# Set a tag on the current branch with the version number found
	git tag "$version" HEAD
	write-host -f green "HEAD tagged with: '$version'"
	
	# Create the zip file containing the built release
	$loc = "D:\Users\Lawrence\Documents\Projects\GW2BuildLibrary\"
	$exeLocation = "bin\Release\GW2BuildLibrary.exe"
	$readmeLocation = "bin\Release\README.pdf"
	$archiveName = "GW2BuildLibrary_$version.zip"

	&"D:\Program Files (x86)\7-Zip\7z.exe" a -tzip $loc$archiveName $loc$exeLocation $loc$readmeLocation
	write-host -f green "Release archive created."
}

if ($Host.Name -eq "ConsoleHost") {
    Write-Host "Press any key to continue..."
    $Host.UI.RawUI.FlushInputBuffer()   # Make sure buffered input doesn't "press a key" and skip the ReadKey().
    $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp") > $null
}