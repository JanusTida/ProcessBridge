using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

// Read and parse the json config file
// Change this to the name of your config file
string? configFile = Path.Combine(AppContext.BaseDirectory,"ProcessBridgeSettings.json"); 
string? cliPath = null;

if(!File.Exists(configFile))
{
    Console.Error.WriteLine($"Config file not found: {configFile}");
    return 1;
}

var json = File.ReadAllText(configFile);
var document = JsonDocument.Parse(json);
var root = document.RootElement;
StreamWriter? logWriter = null;

if (!root.TryGetProperty("cliPath", out JsonElement cliPathElement))
{
    Console.Error.WriteLine("Invalid config file: missing cliPath property.");
    return 1;
}

cliPath = cliPathElement.GetString();
ArgumentNullException.ThrowIfNull(cliPath);
var currentProcessFileName = Environment.GetCommandLineArgs()[0];
cliPath = Path.GetFullPath(cliPath);
if(cliPath == currentProcessFileName)
{
    Console.Error.WriteLine("Invalid config file: cliPath cannot be the same as the path of this program.");
    return 1;
}

if
(
    root.TryGetProperty("logPath",out JsonElement logPathElement) && 
    logPathElement.GetString() is string logPath
)
{
    logWriter = new StreamWriter(logPath);
    logWriter.AutoFlush = true;
}

// Create a process to run the cli program
using var cli = new Process();
// Change this to the path of the cli program
cli.StartInfo.FileName = cliPath; 

logWriter?.WriteLine($"Arguments:{string.Join(' ',args)}");
// Add each argument to the ArgumentList property
foreach (string arg in args) 
{
    cli.StartInfo.ArgumentList.Add(arg);
}

cli.StartInfo.UseShellExecute = false;
cli.StartInfo.RedirectStandardInput = true; 
cli.StartInfo.RedirectStandardOutput = true;
cli.StartInfo.RedirectStandardError = true; 
cli.StartInfo.WorkingDirectory = Environment.CurrentDirectory;

// Subscribe to the event that handles the output data from the cli program
cli.OutputDataReceived += (_,e) =>
{
    logWriter?.WriteLine($"Output {DateTimeOffset.Now}:{e.Data}");
    // Write the output data from the cli program to the standard output of this program
    Console.WriteLine(e.Data);
};
// Subscribe to the event that handles the error data from the cli program
cli.ErrorDataReceived += (_,e) =>
{
    logWriter?.WriteLine($"Error {DateTimeOffset.Now}:{e.Data}");
    // Write the error data from the cli program to the standard error of this program
    Console.Error.WriteLine(e.Data);
};

// Start the process and begin reading output and error asynchronously
cli.Start();
cli.BeginOutputReadLine();
cli.BeginErrorReadLine();
cli.EnableRaisingEvents = true;
Task.Run(() => {
    string? input;
    while ((input = Console.ReadLine()) != null)
    {
        logWriter?.WriteLine($"Input {DateTimeOffset.Now}:{input}");
        cli.StandardInput.WriteLine(input);
        if(cli.HasExited)
        {
            break;
        }
    }
});


// Wait for the process to exit and close it
cli.WaitForExit();
logWriter?.Dispose();
logWriter = null;

return cli.ExitCode;
