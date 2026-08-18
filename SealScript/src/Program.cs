using System;

namespace SealScript;

internal static class Program
{
    private const string ScriptPath = "/home/luna-sparkle/RiderProjects/SealScript/SealScript/scripts/script3.seal";
    
    private static void Main(string[] args)
    {
        SealGlobal.ImportExecutingAssembly();

        RunNice();
    }

    private static void RunNice()
    {
        try
        {
            var fn = UserFunction.CreateFromScript(ScriptPath);

            fn.Invoke();
        }
        catch (SealException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(ex.Message);
            Console.ResetColor();
        }
    }
    
    private static void RunDebug()
    {
        var fn = UserFunction.CreateFromScript(ScriptPath);

        fn.Invoke();
    }
}