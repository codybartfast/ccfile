
namespace Fmbm.IO;

using System.Collections.Concurrent;
public class RockFile
{
    const string LockSuffix = ".lck";
    const string BackupSuffix = ".bak";
    const string NewSuffix = ".tmp";

    static readonly ConcurrentDictionary<string, FileLocks> fileLocksDict =
        new ConcurrentDictionary<string, FileLocks>();

    private FileLocks fileLocks;

    public string FilePath { get; }
    internal string LockPath { get; }
    internal string BackupPath { get; }
    internal string NewPath { get; }

    public RockFile(string filePath)
    {
        FilePath = new FileInfo(filePath).FullName;
        LockPath = FilePath + LockSuffix;
        BackupPath = filePath + BackupSuffix;
        NewPath = filePath + NewSuffix;

        fileLocks = fileLocksDict.GetOrAdd(
            FilePath.ToUpperInvariant(),
            _ => new FileLocks());
    }



    class FileLocks { }

}
