
namespace Fmbm.IO;

public class RockFile
{
    const string LockSuffix = ".lck";
    const string BackupSuffix = ".bak";
    const string NewSuffix = ".tmp";

    public RockFile(string filePath)
    {
        FilePath = new FileInfo(filePath).FullName;
        LockPath = FilePath + LockSuffix;
        BackupPath = filePath + BackupSuffix;
        NewPath = filePath + NewSuffix;
    }

    public string FilePath { get; }
    internal string LockPath {get; }
    internal string BackupPath {get; }
    internal string NewPath {get; }
}
