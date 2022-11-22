using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lurker.Common.Extensions;
using Lurker.Common.Models;
using ProcessLurker;

namespace Lurker.Common.Services
{
    public abstract class ServiceBase<TGame>
        where TGame: GameBase
    {
        #region Properties

        protected string ExecutablePath { get; private set; }

        protected abstract string ProcessName { get; }

        protected abstract string OpenLink { get; }

        #endregion

        #region Methods

        public abstract List<TGame> FindGames();

        public async Task<string> InitializeAsync(string executablePath = null)
        {
            if (!string.IsNullOrEmpty(executablePath) && File.Exists(executablePath))
            {
                ExecutablePath = executablePath;
            }
            else
            {
                var runningSteamProcess = Process.GetProcessesByName(ProcessName);
                if (runningSteamProcess.Any())
                {
                    ExecutablePath = runningSteamProcess[0].GetMainModuleFileName();
                }
                else
                {
                    // Open the launcher to get the Process
                    var command = CliWrap.Cli
                                    .Wrap("cmd.exe")
                                    .WithArguments($"/C {OpenLink}");
                    await command.ExecuteAsync();

                    var processService = new ProcessService(ProcessName);
                    var processId = await processService.WaitForProcess(false);
                    var process = Process.GetProcessById(processId);
                    ExecutablePath = process.GetMainModuleFileName();
                }
            }

            return ExecutablePath;
        }

        #endregion
    }
}
