param(
	[string] $BinaryPath,
	[string] $WinSvcName = 'ITSS.Repository.Gateway',
	[string] $WinSvcDisplayName = 'Quest ITSS Repository Cloud Gateway'
)

$defaultExecName = 'ITSS.Repository.Gateway.exe'
if (-Not $BinaryPath) {
	$BinaryPath = Join-Path $PSScriptRoot $defaultExecName
}

Write-Host "Installing '$WinSvcName' service with binaryPath=$BinaryPath"

$custRegSvc = New-Service -Name "$WinSvcName" -BinaryPathName "$BinaryPath" -DisplayName "$WinSvcDisplayName" -StartupType Automatic
$custRegSvc = Get-Service -Name "$WinSvcName"
if ($custRegSvc)
{
    Write-Host "Successfully created windows service '$WinSvcName'. Starting..."
    $custRegSvc.Start()
    Write-Host "Successfully started windows service '$WinSvcName'"
}
