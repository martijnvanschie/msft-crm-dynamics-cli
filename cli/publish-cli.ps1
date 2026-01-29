### Run the following command to enable execution of unsigned scripts in current sessions.
### Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope Process
###

$publishingfolder = "C:\apps\Azure utils\Microsoft Dynamics CLI"
$singlefile = "true"

# Publish the app
dotnet publish `
    .\Microsoft.Dynamics.Cli\Microsoft.Dynamics.Cli.csproj `
    -p:PublishSingleFile=$singlefile `
    --output $publishingfolder `
    --runtime win-x64 `
    --configuration "Release" `
    --no-self-contained

# Symlink paths
$symlinkPath = "C:\apps\Azure utils\mdcli.exe"
$targetPath  = "C:\apps\Azure utils\Microsoft Dynamics CLI\mdcli.exe"

# Remove existing symlink or file
if (Test-Path $symlinkPath) {
    $item = Get-Item $symlinkPath -Force

    if ($item.LinkType -eq "SymbolicLink") {
        Remove-Item $symlinkPath -Force
    }
    else {
        throw "Cannot create symlink: '$symlinkPath' exists and is not a symlink."
    }
}

# Create new symlink
New-Item -ItemType SymbolicLink -Path $symlinkPath -Target $targetPath | Out-Null
