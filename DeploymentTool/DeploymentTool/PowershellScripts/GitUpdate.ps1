function GitUpdate
{
    Param(
        [parameter(Mandatory=$true)]
        [string]$currentDirectory,
        [parameter(Mandatory=$true)]
        [string]$branch,
        [parameter(Mandatory=$true)]
        [string]$repoName,
  		[parameter(Mandatory=$true)]
        [string]$userName,
		[parameter(Mandatory=$true)]
        [string]$password,
		[parameter(Mandatory=$true)]
        [string]$websiteName

)
    process
    {
        cd $currentDirectory\$websiteName
        git pull https://"$userName":"$password"@bitbucket.org/apollobytes/$reponame $branch -q
    }
}
