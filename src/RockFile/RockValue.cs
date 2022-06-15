namespace Fmbm.IO;

public class RockValue<TValue> : IRockValue<TValue>
{
    internal RockFile RockFile { get; }
    public string Path { get; }

    public RockValue(
        string filePath,
        Func<TValue> getInitialValue,
        Action<string, string?>? archive = null)
        : this(filePath, archive)
    {
        if (!File.Exists(Path))
        {
            TValue initialValue;
            try
            {
                initialValue = getInitialValue();
            }
            catch (Exception ex)
            {
                throw new RockFileGetInitialValueException(ex);
            }
            Write(initialValue);
        }
    }

    public RockValue(string filePath, Action<string, string?>? archive = null)
    {
        this.RockFile = new RockFile(filePath, archive);
        this.Path = RockFile.Path;
    }

    // hide RockFile
    // filepaht -> path

    public TValue ReadOrWrite(Func<TValue> getValue)
    {
        return RockFile.ReadOrWriteValue(getValue);
    }

    public void Modify(Func<TValue?, TValue> modify)
    {
        RockFile.ModifyValue<TValue>(modify);
    }

    public TValue? Read()
    {
        return RockFile.ReadValue<TValue>();
    }

    public void Write(TValue value)
    {
        RockFile.WriteBytes(RockFile.ValueToBytes<TValue>(value));
    }
}
