### Deployment tools and scripts for QA environment

 - Open C:\Users\test\AppData\Roaming\NuGet folder
 - add following packageSources:
 - <add key="Hanseaticsoft" value="https://www.myget.org/F/hanseaticsoft/auth/707dca70-d726-49f8-98aa-233f2120325b/api/v3/index.json" />
 - Copy Nuget exe in Windows Folder
 - Run as Administrator powershell ISE
 - if build is failed make sure to remove all data from C:\TFS\NugetPackages\Packages Folder
 - check if branchdatabase.txt exist