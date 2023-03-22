using namespace System.IO;
param ([string]$ProjectDir, [string]$TargetDir,[string]$Configuration)
Import-Module ./IOUtils.psm1 -Force

$ErrorActionPreference = "Stop";
Write-Host "Executing Powershell pre building...";

Write-Host ($Msg = "IOUtils:" + $PSScriptRoot + "/IOUtils.psm1");
Write-Host ($Msg = "ProjectDir:"+ $ProjectDir);
Write-Host ($Msg = "TargetDir:" + $TargetDir);
Write-Host ($Msg = "Configuration:" + $Configuration);

#Build ProcessBridge and ProcessBridgeTest.Cli;
dotnet build $ProjectDir/../ProcessBridge/ProcessBridge.csproj -c $Configuration
dotnet build $ProjectDir/../ProcessBridgeTest.Cli/ProcessBridgeTest.Cli.csproj -c $Configuration

Copy-DirectoryEx $ProjectDir/../ProcessBridge/bin/$Configuration/net7.0/ $TargetDir
Copy-DirectoryEx $ProjectDir/../ProcessBridgeTest.Cli/bin/$Configuration/net7.0/ $TargetDir

$ProcessBridgeSettingsJsonFile = $ProjectDir + "ProcessBridgeSettings.json";
$ProcessBridgeSettingsJsonFileTarget = $TargetDir + "ProcessBridgeSettings.json";
#Copy ProcessBridgeSettings.json to target directory;
Copy-FileEx  $ProcessBridgeSettingsJsonFile $ProcessBridgeSettingsJsonFileTarget

#If the OS is Windows, add ".exe" to the cliPath;
if ($env:OS -match "Windows_NT")
{
    $json = Get-Content $ProcessBridgeSettingsJsonFileTarget | ConvertFrom-Json
    $json.cliPath += ".exe"
    $json | ConvertTo-Json | Set-Content $ProcessBridgeSettingsJsonFileTarget
} 



