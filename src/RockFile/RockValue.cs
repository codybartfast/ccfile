namespace Fmbm.IO;

public class RockValue<TValue> : IRockValue<TValue>
{
    internal RockFile RockFile { get; }
    public string FilePath { get; }

    public RockValue(
        string filePath,
        Func<TValue> getInitialValue,
        RockFileArchive? archive = null)
        : this(filePath, archive)
    {
        if (!File.Exists(FilePath))
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

    public RockValue(string filePath, RockFileArchive? archive = null)
    {
        this.RockFile = new RockFile(filePath, archive);
        this.FilePath = RockFile.FilePath;
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
