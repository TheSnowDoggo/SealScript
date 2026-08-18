namespace SealScript;

public interface ILineNumbered
{
    int Line { get; }
    int Column { get; }
}