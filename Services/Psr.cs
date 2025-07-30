using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace AutoPBI.Services;

public class Psr : IDisposable
{
    private readonly Runspace _runspace;

    public Psr()
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
    }

    public CommandBuilder Wrap(string executablePath = null)
    {
        return new CommandBuilder(this, executablePath);
    }

    internal async Task<CommandResult> ExecutePowerShellCommandAsync(string command, Action<string> outputHandler = null, Action<string> errorHandler = null)
    {
        var result = new CommandResult();
        using (var pipeline = _runspace.CreatePipeline())
        {
            pipeline.Commands.AddScript(command);

            // Capture errors
            pipeline.Error.DataReady += (sender, e) =>
            {
                while (pipeline.Error.Count > 0)
                {
                    var error = pipeline.Error.Read() as ErrorRecord;
                    if (error != null)
                    {
                        var errorMessage = error.ToString();
                        result.Error.Add(errorMessage);
                        errorHandler?.Invoke(errorMessage);
                    }
                }
            };

            var results = await Task.Run(() => pipeline.Invoke());

            // Capture PSObjects
            result.Objects.AddRange(results);

            // Capture output as strings
            foreach (var obj in results)
            {
                var output = obj?.ToString() ?? "";
                if (!string.IsNullOrEmpty(output))
                {
                    result.Output.Add(output);
                    outputHandler?.Invoke(output);
                }
            }

            return result;
        }
    }

    internal async Task<CommandResult> ExecuteExternalCommandAsync(string executablePath, string arguments, Action<string> outputHandler = null, Action<string> errorHandler = null)
    {
        var result = new CommandResult();
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                result.Output.Add(e.Data);
                outputHandler?.Invoke(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                result.Error.Add(e.Data);
                errorHandler?.Invoke(e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await Task.Run(() => process.WaitForExit());

        return result;
    }

    public void Dispose()
    {
        _runspace.Close();
        _runspace.Dispose();
    }
}

public class CommandResult
{
    public List<string> Error { get; } = new List<string>();
    public List<PSObject> Objects { get; } = new List<PSObject>();
    public List<string> Output { get; } = new List<string>();
}

public class CommandBuilder
{
    private readonly Psr _service;
    private readonly string _executablePath;
    private List<string> _arguments = new List<string>();
    private Action<string> _outputHandler;
    private Action<string> _errorHandler;

    public CommandBuilder(Psr service, string executablePath = null)
    {
        _service = service;
        _executablePath = executablePath;
    }

    public CommandBuilder WithArguments(Action<ArgumentBuilder> action)
    {
        var argBuilder = new ArgumentBuilder();
        action(argBuilder);
        _arguments.AddRange(argBuilder.Arguments);
        return this;
    }

    public CommandBuilder WithStandardOutputPipe(Action<string> outputHandler)
    {
        _outputHandler = outputHandler;
        return this;
    }

    public CommandBuilder WithStandardErrorPipe(Action<string> errorHandler)
    {
        _errorHandler = errorHandler;
        return this;
    }

    public async Task<CommandResult> ExecuteAsync()
    {
        if (_arguments.Count == 0)
            throw new InvalidOperationException("At least one argument (command) must be specified before execution.");

        var command = _arguments[0];
        var remainingArguments = _arguments.Skip(1).Select(Escape);
        var arguments = string.Join(" ", remainingArguments);

        if (!string.IsNullOrEmpty(_executablePath))
        {
            // Run external executable
            return await _service.ExecuteExternalCommandAsync(_executablePath, $"{Escape(command)} {arguments}", _outputHandler, _errorHandler);
        }

        // Run PowerShell command
        var fullCommand = string.IsNullOrEmpty(arguments) ? command : $"{command} {arguments}";
        return await _service.ExecutePowerShellCommandAsync(fullCommand, _outputHandler, _errorHandler);
    }

    private static string Escape(string argument)
    {
        if (string.IsNullOrEmpty(argument))
            return "\"\"";

        if (argument.Length > 0 && argument.All(c => !char.IsWhiteSpace(c) && c != '"'))
            return argument;

        var buffer = new StringBuilder();
        buffer.Append('"');

        for (var i = 0; i < argument.Length; )
        {
            var c = argument[i++];

            if (c == '\\')
            {
                var backslashCount = 1;
                while (i < argument.Length && argument[i] == '\\')
                {
                    backslashCount++;
                    i++;
                }

                if (i == argument.Length)
                {
                    buffer.Append('\\', backslashCount * 2);
                }
                else if (i == argument.Length || argument[i] == '"')
                {
                    buffer.Append('\\', backslashCount * 2 + 1).Append('"');
                    i++;
                }
                else
                {
                    buffer.Append('\\', backslashCount);
                }
            }
            else if (c == '"')
            {
                buffer.Append('\\').Append('"');
            }
            else
            {
                buffer.Append(c);
            }
        }

        buffer.Append('"');
        return buffer.ToString();
    }
}

public class ArgumentBuilder
{
    public List<string> Arguments { get; } = new List<string>();

    public ArgumentBuilder Add(params string[] args)
    {
        Arguments.AddRange(args);
        return this;
    }
}