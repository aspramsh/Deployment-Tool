function publishPrj
{
    Param(
    [parameter(Mandatory=$true)]
    [string]$projectPath,
    [parameter(Mandatory=$true)]
    [string]$publishPath
)
process
    {
        dotnet publish $projectPath --configuration Release --force --output $publishPath
    }
}
