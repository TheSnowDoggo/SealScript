using System.Collections.Generic;
using System.Collections;

namespace SealScript;

[SealObjectExport(nameof(ClassInstance), Name = "Range")]
public class SealRange : SealObject, IEnumerable<SealValue>
{
    private readonly IEnumerable<SealValue> _values;
    
    public SealRange(IEnumerable<SealValue> values)
    {
        _values = values;
    }
    
    static SealRange()
    {
        ClassInstance = ClassFactory<SealRange>.Generate();
    }

    public static readonly SealClass ClassInstance;

    public override SealClass Class => ClassInstance;

    public static IEnumerable<SealValue> CreateRange(double start, double end, double step)
    {
        switch (step)
        {
            case 0:
                yield break;
            case > 0:
            {
                for (double i = start; i < end; i += step)
                {
                    yield return i;
                }

                break;
            }
            default:
            {
                for (double i = start; i > end; i += step)
                {
                    yield return i;
                }

                break;
            }
        }
    }

    public static IEnumerable<SealValue> CreateRange(double start, double end)
    {
        return CreateRange(start, end, end >= start ? 1 : -1);
    }
    
    public static IEnumerable<SealValue> CreateRange(double end)
    {
        return CreateRange(0, end, end >= 0 ? 1 : -1);
    }
    
    public IEnumerator<SealValue> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}