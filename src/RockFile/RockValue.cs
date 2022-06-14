namespace Fmbm.IO;

public class RockValue<TValue> : IRockValueFile<TValue>
{
    public RockFile RockFile { get; }
    public string FilePath { get; }

    public RockValue(string filePath, RockFileArchive? archive = null)
        : this(new RockFile(filePath, archive)) { }

    public RockValue(RockFile rockFile)
    {
        this.RockFile = rockFile;
        this.FilePath = rockFile.FilePath;
    }

    public void Modify(Func<TValue?, TValue> modify)
    {
        RockFile.ModifyValue<TValue>(modify);
    }

    public TValue? Read()
    {
        return RockFile.ReadValue<TValue>();
    }

    public void Write(TValue obj)
    {
        RockFile.WriteBytes(RockFile.ValueToBytes<TValue>(obj));
    }
}
