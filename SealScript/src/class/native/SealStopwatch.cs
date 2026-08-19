using System.Diagnostics;

namespace SealScript;

[SealObjectExport(nameof(ClassInstance), Name = "Stopwatch")]
public class SealStopwatch : SealObject
{
    private readonly Stopwatch _sw = new Stopwatch();
    
    static SealStopwatch()
    {
        ClassInstance = ClassFactory<SealStopwatch>.Generate();
    }
    
    public static readonly SealClass ClassInstance;

    public override SealClass Class => ClassInstance;
    
    [PropertyExport]
    public SealValue IsRunning => _sw.IsRunning;

    [PropertyExport]
    public SealValue ElapsedDays => _sw.Elapsed.TotalDays;
    
    [PropertyExport]
    public SealValue ElapsedHours => _sw.Elapsed.TotalHours;
    
    [PropertyExport]
    public SealValue ElapsedMinutes => _sw.Elapsed.TotalMinutes;
    
    [PropertyExport]
    public SealValue ElapsedSeconds => _sw.Elapsed.TotalSeconds;
    
    [PropertyExport]
    public SealValue ElapsedMilliseconds => _sw.Elapsed.TotalMilliseconds;
    
    [PropertyExport]
    public SealValue ElapsedMicroseconds => _sw.Elapsed.TotalMicroseconds;
    
    [PropertyExport]
    public SealValue ElapsedNanoseconds => _sw.Elapsed.TotalNanoseconds;

    [FunctionExport]
    public static SealValue New(CallArgs args)
    {
        return new SealStopwatch();
    }
    
    [FunctionExport]
    public void Start(CallArgs args)
    {
        _sw.Start();
    }

    [FunctionExport]
    public void Stop(CallArgs args)
    {
        _sw.Stop();
    }

    [FunctionExport]
    public void Restart(CallArgs args)
    {
        _sw.Restart();
    }
    
    [FunctionExport]
    public void Reset(CallArgs args)
    {
        _sw.Reset();
    }
}