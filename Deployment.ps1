function buildVS
{
    param
    (
        [parameter(Mandatory=$true)]
        [String] $RepoName = $( Read-Host),

        [parameter(Mandatory=$true)]
        [String] $BranchName = $( Read-Host),

        [parameter(Mandatory=$true)]
        [String] $Configuration = $( Read-Host),
        
        [parameter(Mandatory=$true)]
        [String] $WebSiteName = $( Read-Host)
    )
    process
    {


        if(get-process | ?{$_.path -eq "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe"})
        {
            Write-Host "Visual Studio is running no need to run again"
        }
        else 
        {
            & "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe"
        }


        $branchpath = "C:\Projects\Hanseaticsoft\$RepoName"



        cd $branchpath
        git checkout .

        if(git rev-parse --abbrev-ref HEAD -eq $BranchName)
        {
            Write-Host "Already checked out correctly branch"
        }
        else
        {
            git checkout $BranchName
        }
        #git pull


        $SolutionPath = "$branchpath\CloudFleetManager"

        $SolutionFilePath  = "$SolutionPath\HS.CloudFleetmanager.sln"

        $msBuildExe = 'C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe'

        New-PSDrive -name T -psprovider FileSystem -root C:\TFS\NugetPackages

        
        
        

        $copyPath = "C:\ProjectFiles\HS.CloudFleetmanager.sln"
        Get-Item -Path $copyPath
        Copy-Item  -Path $copyPath -Destination $SolutionPath -Force

        (Get-Content "$SolutionPath\HS.CFM.WebBase\Initialization\AzureEnvironment.cs").replace('https://local.hanseaticloft.com', "https://$WebSiteName.hanseaticloft.com" ) | Set-Content "$SolutionPath\HS.CFM.WebBase\Initialization\AzureEnvironment.cs"


        (Get-Content "$SolutionPath\HS.CloudShipping.Hosting\HS.CloudShipping.Hosting.csproj").replace('https://local.hanseaticloft.com', "https://$WebSiteName.hanseaticloft.com" ) | Set-Content "$SolutionPath\HS.CloudShipping.Hosting\HS.CloudShipping.Hosting.csproj"

        cd $SolutionPath
        ls -Recurse -include 'bin','obj','packages' |
        foreach {
           if ((ls $_.Parent.FullName | ?{ $_.Name -Like "*.sln" -or $_.Name -Like "*.*proj" }).Length -gt 0) {
              remove-item $_ -recurse -force
              write-host deleted $_
            }
        }

        
        #Restoring Nuget Packages


            $NUGETLOCATION = "C:\TFS\NuGet.exe"
        
        #Write-Host "Starting Package Restore - This may take a few minutes ..."
		#$PACKAGECONFIGS = Get-ChildItem -Recurse -Force $SolutionPath -ErrorAction SilentlyContinue | 
		#Where-Object { ($_.PSIsContainer -eq $false) -and  ( $_.Name -eq "packages.config")}
		#ForEach($PACKAGECONFIG in $PACKAGECONFIGS)
		#	{
		#		Write-Host $PACKAGECONFIG.FullName
		#		$NugetRestore = $NUGETLOCATION + " install " + " '" + $PACKAGECONFIG.FullName + "' -OutputDirectory 'C:\TFS\NugetPackages\packages'"
		#		Write-Host $NugetRestore
		#		Invoke-Expression $NugetRestorez
		#	}
        

        Write-Host "Restoring NuGet packages $SolutionFilePath" -foregroundcolor green
        nuget restore  $SolutionFilePath
        

        #end of NugetPackage Restore
            

            Write-Host "Building $($SolutionFilePath)" -foregroundcolor green
        & "$($msBuildExe)" "$($SolutionFilePath)"  /t:Build  /m /p:Configuration="$($Configuration)" /p:PreBuildEvent="echo test output" /p:PostBuildEvent=""
    }
    }


 buildVS 
