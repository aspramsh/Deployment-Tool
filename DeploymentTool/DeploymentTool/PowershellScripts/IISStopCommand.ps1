Param(
    [parameter(Mandatory=$true)]
    [string]$poolName
 )
 Stop-WebAppPool -Name $poolName