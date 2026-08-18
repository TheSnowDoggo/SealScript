using System;

namespace SealScript;

internal static class Program
{
    private static void Main(string[] args)
    {
        SealGlobal.ImportExecutingAssembly();

        if (args.Length < 1)
        {
            PrintError("Expected first argument to be a filepath.");
            return;
        }

        string filepath = args[0];
        
        try
        {
            var fn = UserFunction.CreateFromScript(filepath);

            fn.Invoke();
        }
        catch (Exception ex)
        {
            PrintError(ex.Message);
        }

        Console.Read();
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(message);
        Console.ResetColor();
    }
}