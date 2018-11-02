Param(
    [parameter(Mandatory=$true)]
    [string]$poolName
 )
 Start-WebAppPool -Name $poolName