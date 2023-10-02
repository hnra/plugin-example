namespace MyLib;

using System;

using Plugin;

public class ClassLib : IPlugin
{
    public void Execute(string msg)
    {
        Console.WriteLine($"New: {msg}");
    }
}