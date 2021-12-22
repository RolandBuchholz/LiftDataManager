using LiftDataManager.Core.Contracts.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LiftDataManager.Core.Services
{
    public class VaultDataService : IVaultDataService
    {
        const string pathPowershellScripts = @"C:\Work\Administration\PowerShellScripts\";

        public async Task<int> GetFileAsync(string auftragsnummer)
        {
            string powershellScriptName;
            powershellScriptName = "GetVaultFile.ps1";

            try
            {
                using Process getFile = new();
                getFile.StartInfo.UseShellExecute = false;
                getFile.StartInfo.FileName = "PowerShell.exe";
                getFile.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer}";
                getFile.StartInfo.CreateNoWindow = false;
                getFile.Start();
                getFile.WaitForExit();

                Debug.WriteLine("GetVaultFile finished ......");
                await Task.CompletedTask;
                return getFile.ExitCode;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                await Task.CompletedTask;
                return 4;
            }
        }

        public async Task<int> SetFileAsync(string auftragsnummer)
        {
            string powershellScriptName;
            powershellScriptName = "SetVaultFile.ps1";

            try
            {
                using Process setFile = new();
                setFile.StartInfo.UseShellExecute = false;
                setFile.StartInfo.FileName = "PowerShell.exe";
                setFile.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer}";
                setFile.StartInfo.CreateNoWindow = false;
                setFile.Start();
                setFile.WaitForExit();

                Debug.WriteLine("SetVaultFile finished ......");
                await Task.CompletedTask;
                return setFile.ExitCode;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                await Task.CompletedTask;
                return 4;
            }
        }
    }
}
