using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace AutoPBI.Services
{
    public class PowerShellOutputCollection : ConcurrentBag<string>
    {
        public override string ToString() => string.Join(Environment.NewLine, this);
    }

    public class PowerShellErrorCollection : ConcurrentBag<string>
    {
        public override string ToString() => string.Join(Environment.NewLine, this);
    }

    public class PowerShellHostCollection : ConcurrentBag<string>
    {
        public override string ToString() => string.Join(Environment.NewLine, this);
    }

    public class PsResult
    {
        public PowerShellHostCollection Host { get; } = new();
        public PowerShellOutputCollection Output { get; } = new();
        public PowerShellErrorCollection Error { get; } = new();
        public List<PSObject> Objects { get; } = new();

        public void Write()
        {
            var needsNewline = false;

            if (Host.Count > 0)
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                foreach (var line in Host)
                {
                    Console.WriteLine(line);
                }
                Console.ForegroundColor = originalColor;
                needsNewline = true;
            }

            if (Output.Count > 0)
            {
                if (needsNewline) Console.WriteLine();
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.White;
                foreach (var line in Output)
                {
                    Console.WriteLine(line);
                }
                Console.ForegroundColor = originalColor;
                needsNewline = true;
            }

            if (Error.Count > 0)
            {
                if (needsNewline) Console.WriteLine();
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var line in Error)
                {
                    Console.WriteLine(line);
                }
                Console.ForegroundColor = originalColor;
            }
        }
    }

    public sealed class PsRunner : IDisposable
    {
        private readonly Runspace _runspace;
        private readonly PowerShell _powershell;
        private bool _disposed;

        public PsRunner()
        {
            var initialSessionState = InitialSessionState.CreateDefault();
            initialSessionState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Bypass;

            // Get both Windows PowerShell and PowerShell Core module paths
            var windowsPsModulePath = Environment.ExpandEnvironmentVariables(@"%UserProfile%\Documents\WindowsPowerShell\Modules");
            var psCoreModulePath = Environment.ExpandEnvironmentVariables(@"%UserProfile%\Documents\PowerShell\Modules");
            const string systemModulePath = @"C:\Program Files\WindowsPowerShell\Modules";
            const string psCoreProgramFilesPath = @"C:\Program Files\PowerShell\Modules";

            var modulePaths = new[] 
            { 
                windowsPsModulePath,
                psCoreModulePath,
                systemModulePath,
                psCoreProgramFilesPath
            };

            var currentModulePath = Environment.GetEnvironmentVariable("PSModulePath", EnvironmentVariableTarget.Process) ?? "";
            var newModulePath = string.Join(";", modulePaths.Concat(currentModulePath.Split(';')));
            
            initialSessionState.EnvironmentVariables.Add(new SessionStateVariableEntry("PSModulePath", newModulePath, ""));

            _runspace = RunspaceFactory.CreateRunspace(initialSessionState);
            _runspace.Open();

            _powershell = PowerShell.Create();
            _powershell.Runspace = _runspace;
        }

        public async Task<PsResult> Execute(params string[] scripts)
        {
            return await Task.Run(() =>
            {
                var result = new PsResult();

                _powershell.Commands.Clear();
                _powershell.Streams.ClearStreams();

                _powershell.AddScript("$ErrorActionPreference = 'Continue'");
                var combinedScript = string.Join("\n", scripts);
                _powershell.AddScript(combinedScript);

                _powershell.Streams.Error.DataAdded += (sender, e) =>
                {
                    var errorRecord = ((PSDataCollection<ErrorRecord>)sender!)[e.Index];
                    var lines = errorRecord?.ToString().Split(["\r\n", "\n"], StringSplitOptions.None);
                    if (lines != null) 
                    {
                        foreach (var line in lines)
                        {
                            result.Error.Add(line);
                        }
                    }
                };

                _powershell.Streams.Information.DataAdded += (sender, e) =>
                {
                    var informationRecord = ((PSDataCollection<InformationRecord>)sender!)[e.Index];
                    var lines = informationRecord?.MessageData.ToString()?.Split(["\r\n", "\n"], StringSplitOptions.None);
                    if (lines != null)
                    {
                        foreach (var line in lines)
                        {
                            result.Host.Add(line);
                        }
                    }
                };

                var psResults = _powershell.Invoke();

                foreach (var psResult in psResults)
                {
                    if (psResult == null) continue;
                    result.Objects.Add(psResult);
                    var lines = psResult.ToString().Split(["\r\n", "\n"], StringSplitOptions.None);
                    foreach (var line in lines)
                    {
                        result.Output.Add(line);
                    }
                }

                return result;
            });
        }

        public async Task SetDirectory(string path)
        {
            await Execute($"Set-Location -Path '{path.Replace("'", "''")}'");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            
            if (disposing)
            {
                _powershell.Dispose();
                _runspace.Dispose();
            }

            _disposed = true;
        }

        ~PsRunner()
        {
            Dispose(false);
        }
    }
}