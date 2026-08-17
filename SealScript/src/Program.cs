namespace SealScript;

internal static class Program
{
    private const string ScriptPath = "/home/luna-sparkle/RiderProjects/SealScript/SealScript/scripts/script3.seal";
    
    private static void Main(string[] args)
    {
        SealGlobal.ImportExecutingAssembly();
        
        var fn = UserFunction.CreateFromScript(ScriptPath);

        fn.Invoke();
    }
}