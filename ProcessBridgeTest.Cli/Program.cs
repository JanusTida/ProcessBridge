using System;

Console.WriteLine("Cli started.");
Console.WriteLine($"Current directory:{Environment.CurrentDirectory}");
Console.WriteLine($"Arguments:{string.Join(' ',args)}");

string? line = null;
while((line = Console.ReadLine()) != null)
{
    line = line.Trim();
    if(string.Compare(line,"exit",true) == 0)
    {
        Console.WriteLine("Cli exited.");
        break;
    }
    if(line.ToLower().StartsWith("error"))
    {
        Console.Error.WriteLine($"Cli error:{line}");
    }
    else
    {
        Console.WriteLine($"You input:{line}");
    }
}

return 1;

