namespace Fmbm.IO;

public class CCValue<TValue> : ICCValue<TValue>
{
    internal CCFile CCFile { get; }
    public string Path { get; }

    public CCValue(
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
                throw new CCFileGetInitialValueException(ex);
            }
            Write(initialValue);
        }
    }

    public CCValue(string filePath, Action<string, string?>? archive = null)
    {
        this.CCFile = new CCFile(filePath, archive);
        this.Path = CCFile.Path;
    }

    public TValue ReadOrWrite(Func<TValue> getValue)
    {
        return CCFile.ReadOrWriteValue(getValue);
    }

    public void Modify(Func<TValue?, TValue> modify)
    {
        CCFile.ModifyValue<TValue>(modify);
    }

    public TValue? Read()
    {
        return CCFile.ReadValue<TValue>();
    }

    public void Write(TValue value)
    {
        CCFile.WriteBytes(CCFile.ValueToBytes<TValue>(value));
    }
}
