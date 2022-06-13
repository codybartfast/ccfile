namespace Fmbm.IO;

public class RockFileObject<TObject> : IRockFileObject<TObject>
{
    public RockFile RockFile { get; }
    public string FilePath { get; }


    public RockFileObject(string filePath, RockFileArchive? archive = null)
        : this(new RockFile(filePath, archive)) { }

    public RockFileObject(RockFile rockFile)
    {
        this.RockFile = rockFile;
        this.FilePath = rockFile.FilePath;
    }

    public void Modify(Func<TObject?, TObject> modify)
    {
        RockFile.ModifyObject<TObject>(modify);
    }

    public TObject? Read()
    {
        return RockFile.ReadObject<TObject>();
    }

    public void Write(TObject obj)
    {
        RockFile.WriteBytes(RockFile.ObjectToBytes<TObject>(obj));
    }
}