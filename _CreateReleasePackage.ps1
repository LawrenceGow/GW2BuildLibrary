$version = Read-Host 'New Version'

# Update AssemblyVersion in the project
$infoPath = "Properties\AssemblyInfo.cs"
$regex = '^\[assembly: AssemblyVersion\(".*"\)\]$'
$newVersion = '[assembly: AssemblyVersion("' + $version + '")]'
(Get-Content $infoPath) -replace $regex, $newVersion | Set-Content $infoPath

# Update AssemblyFileVersion in the project
$regex = '^\[assembly: AssemblyFileVersion\(".*"\)\]$'
$newVersion = '[assembly: AssemblyFileVersion("' + $version + '")]'
(Get-Content $infoPath) -replace $regex, $newVersion | Set-Content $infoPath

# Build the project in Release
&"D:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe" "GW2BuildLibrary.sln" /t:Clean,Build /p:Configuration=Release /p:Platform="Any CPU"

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
	# Files to include
	$file1 = "GW2BuildLibrary.exe"
	$file2 = "README.pdf"
	$file3 = "GW2BuildLibrary.exe.config"
	
	$archiveName = "GW2BuildLibrary_$version.zip"

	# Copy files into current directory
	xcopy bin\Release\* . /Y
	
	# Create the archive
	&"D:\Program Files (x86)\7-Zip\7z.exe" a -tzip $archiveName $file1 $file2 $file3
	write-host -f green "Release archive created."
	
	# Undo file change for version bump
	git restore $infoPath
}

if ($Host.Name -eq "ConsoleHost") {
    Write-Host "Press any key to continue..."
    $Host.UI.RawUI.FlushInputBuffer()   # Make sure buffered input doesn't "press a key" and skip the ReadKey().
    $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp") > $null
}