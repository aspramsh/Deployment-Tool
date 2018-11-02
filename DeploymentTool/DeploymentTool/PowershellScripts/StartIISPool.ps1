function StartIISAppPool
{
    Param(
    [parameter(Mandatory=$true)]
    [string]$startScriptPath,
    [parameter(Mandatory=$true)]
    [string]$poolName
)
    process
    {
        PowerShell.exe -NoProfile -ExecutionPolicy Unrestricted -Command "& {Start-Process PowerShell -windowstyle hidden -ArgumentList '-NoProfile -ExecutionPolicy Unrestricted -noexit -File "$startScriptPath -poolName "$poolname""' -Verb RunAs}";
    }
}