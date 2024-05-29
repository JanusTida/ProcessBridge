// See https://aka.ms/new-console-template for more information
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

var bridgeProgramPath = Path.Combine(AppContext.BaseDirectory,"ProcessBridge");
//On Windows, the bridge program is a .exe file
if(RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
{
    bridgeProgramPath += ".exe";
}

if(!File.Exists(bridgeProgramPath))
{
    Console.Error.WriteLine($"Bridge program not found: {bridgeProgramPath}");
    return 1;
}

var bridgeProcess = new Process();
bridgeProcess.StartInfo.FileName = bridgeProgramPath;
bridgeProcess.StartInfo.ArgumentList.Add("This is my argument to cli program.");
bridgeProcess.StartInfo.WorkingDirectory = AppContext.BaseDirectory;
bridgeProcess.StartInfo.UseShellExecute = false;
bridgeProcess.StartInfo.RedirectStandardInput = true;
bridgeProcess.StartInfo.RedirectStandardOutput = true;
bridgeProcess.StartInfo.RedirectStandardError = true;

bridgeProcess.EnableRaisingEvents = true;
bridgeProcess.OutputDataReceived += (_, e) => 
{
    if(e.Data != null)
    {
        Console.WriteLine(e.Data);
    }
};

bridgeProcess.ErrorDataReceived += (_, e) => 
{
    if(e.Data != null)
    {
        Console.Error.WriteLine(e.Data);
    }
};

bridgeProcess.Start();
bridgeProcess.BeginOutputReadLine();
bridgeProcess.BeginErrorReadLine();

_ = Task.Run(() => {
    string? input;
    while ((input = Console.ReadLine()) != null)
    {
        bridgeProcess.StandardInput.WriteLine(input);
        if(bridgeProcess.HasExited)
        {
            break;
        }
    }
});

bridgeProcess.WaitForExit();
return bridgeProcess.ExitCode;