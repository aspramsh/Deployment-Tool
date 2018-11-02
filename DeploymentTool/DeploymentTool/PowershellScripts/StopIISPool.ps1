function StopIISAppPool
{
    Param(
    [parameter(Mandatory=$true)]
    [string]$stopScriptPath,
    [parameter(Mandatory=$true)]
    [string]$poolName
)
    process
    {
        PowerShell.exe -NoProfile -ExecutionPolicy Unrestricted -Command "& {Start-Process PowerShell -windowstyle hidden -ArgumentList '-NoProfile -ExecutionPolicy Unrestricted -noexit -File $stopScriptPath -poolName $poolname' -Verb RunAs}";
    }
}