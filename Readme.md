## What is ProcessBridge?
ProcessBridge is pretty simple tool that allows you to capture the standard input/output/error stream between a invoker program (We will call it invoker below) and  the program invoker invoked (We will call it cli below).
this tool could be useful when you want to know the input and output contents between two console programs,especially when you don't have the source code of invoker or cli.

The input/output/error will be written to a log file,so you can read the file to inspect what happened between invoker and cli,the paths of cli and log file are configurable,these configurations are configured in a json file called ProcessBridgeSettings.json:
```json
{
    "cliPath":"Your path to the CLI program",
    "logPath":"Your path to the log file"
}
```
## Test projects
There are two test projects inside the solution called ProcessBridgeTest.Cli and ProcessBridgeTest.Invoker,build ProcessBridgeTest.Invoker,the PostBuild.ps1 of this project will build another two projects and copy the outputs into the target folder. [So please ensure that you have powershell installed on your system before you build this project](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.3),and ProcessBridgeSettings.json inside the project will override the previous one from project ProcessBridge,its content is shown below:

```json
{
  "cliPath": "ProcessBridgeTest.Cli",
  "logPath": "ProcessBridge.log"
}
```
`please note that the property "cliPath" will be appended with ".exe" on windows`

Now run ProcessBridgeTest.Invoker,the project will start ProcessBridge and the ProcessBridge will start the ProcessBridgeTest.Cli,type some text to see how it works:


<span style="font-weight:bold">
Cli started.
</span>

<span style="font-weight:bold">
Current directory:/home/janus/Code Repository/ProcessBridge/ProcessBridgeTest.Invoker/bin/Debug/net7.0
</span>

<span style="font-weight:bold">
Arguments:This is my argument to cli program.    
</span>

<span style="color:blue;font-weight:bold">Hello world</span>      
<span style="font-weight:bold">You input:Hello world</span>   
<span style="color:blue;font-weight:bold">Error happened</span>    
<span style="color:red;font-weight:bold">Cli error:Error happened</span>      
<span style="color:blue;font-weight:bold">exit</span>  
<span style="font-weight:bold">Cli exited.</span>

