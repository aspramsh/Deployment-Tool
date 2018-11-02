using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Publisher
{
    public class ProjectPublisher
    {

        /// <summary>
        /// A function that publishes a given project to a given folder
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="scriptPath"></param>
        /// <param name="finalDeploymentPath"></param>
        /// <param name="foldersNotToDelete"></param>
        /// <param name="websiteName"></param>
        /// <returns></returns>
        public static bool PublishToAFolder(Paths paths, string scriptPath, string finalDeploymentPath, FoldersNotToDelete foldersNotToDelete, string websiteName)
        {
            if (!Directory.Exists(paths.Solution))
            {
                throw new Exception(Constants.ProjectFolderNotExist);
            }

            String todaysdate = DateTime.Now.ToString("dd-MMM-yyyy");
            string directoryPath = $"{paths.Publish}\\{websiteName}_{todaysdate}_{DateTime.Now.Ticks}";
            Directory.CreateDirectory(directoryPath);

            ManageIisApplicationPool(scriptPath, paths.IISAppPoolName, "stop");
            
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript("Set-ExecutionPolicy -Scope LocalMachine Unrestricted");
                string publishCommand = File.ReadAllText($"{scriptPath}PublishProject.ps1");
                ps.AddScript(publishCommand);
                ps.AddScript($"publishPrj -projectPath {paths.Solution} -publishPath {directoryPath}");
                ps.Invoke();

                Collection<ErrorRecord> errors = ps.Streams.Error.ReadAll();

                if (errors.Count == 0)
                {
                    if (!Directory.Exists(finalDeploymentPath))
                    {
                        throw new Exception(Constants.PublishFolderNotExist);
                    }
                    DirectoryInfo directoryInfo = new DirectoryInfo(finalDeploymentPath);
                    foreach (FileInfo file in directoryInfo.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
                    {
                        if (dir.Name != foldersNotToDelete.Name)
                        {
                            dir.Delete(true);
                        }
                    }
                    var tempDirectoryInfo = new DirectoryInfo(directoryPath);
                    var finalDirectoryInfo = new DirectoryInfo(finalDeploymentPath);
                    CopyFilesRecursively(tempDirectoryInfo, finalDirectoryInfo);
                    ManageIisApplicationPool(scriptPath, paths.IISAppPoolName, "start");
                    return true;
                }
                string errorToDisplay = null;
                
                foreach (var error in errors)
                {
                    errorToDisplay += $"{error.Exception.Message} ";
                }
                throw new Exception(errorToDisplay);
            }
        }

        private static void ManageIisApplicationPool(string scriptPath, string poolName, string action)
        {
            Process process = new Process();
            string cmdPathStop = $"{scriptPath}{poolName}_{action}.cmd";
            string stopCommand = "C:\\Windows\\System32\\inetsrv\\appcmd " +
                                 $"{action} apppool /apppool.name:{poolName}";
            if (!File.Exists(cmdPathStop))
            {
                File.Create(cmdPathStop).Dispose();
                using (var tw = new StreamWriter(cmdPathStop))
                {
                    tw.WriteLine(stopCommand);
                    tw.Close();
                }
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = cmdPathStop,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true,
                Verb = "runas"
            };
            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();
        }

        /// <summary>
        /// A function that gets project from bitbucker repo
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="scriptPath"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetFromRepository(Params parameters, string scriptPath, UserAccounts account)
        {
            string scriptForUpdating = File.ReadAllText($"{scriptPath}GitUpdate.ps1");

            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript("Set-ExecutionPolicy -Scope LocalMachine Unrestricted");
                ps.AddScript(scriptForUpdating);
                ps.AddScript($"GitUpdate -CurrentDirectory {parameters.ProjectPath} -branch {parameters.BranchName} -repoName {parameters.RepoName} -userName {account.UserName} -password {account.Password} -websiteName {parameters.WebsiteName}");
                ps.Invoke();
                Collection<ErrorRecord> errors = ps.Streams.Error.ReadAll();

                if (errors.Count == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public static void RunSqlScript(string connectionString, string sqlScriptPath)
        {
            string sqlScript = File.ReadAllText($"{sqlScriptPath}");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                Server server = new Server(new ServerConnection(conn));

                server.ConnectionContext.ExecuteNonQuery(sqlScript);
            }
            
        }

        /// <summary>
        /// Copies files and folders from source directory to target directory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
    }
}
