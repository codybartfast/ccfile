
namespace Fmbm.IO;

using System.Collections.Concurrent;

public class RockFile
{
    const string LockSuffix = ".lck";
    const string BackupSuffix = ".bak";
    const string NewSuffix = ".tmp";

    static readonly ConcurrentDictionary<string, object> fileLockDict =
        new ConcurrentDictionary<string, object>();

    private readonly object fileLock;

    public string FilePath { get; }
    internal string LockPath { get; }
    internal string BackupPath { get; }
    internal string TempPath { get; }

    public RockFile(string filePath)
    {
        FilePath = new FileInfo(filePath).FullName;
        LockPath = FilePath + LockSuffix;
        BackupPath = filePath + BackupSuffix;
        TempPath = filePath + NewSuffix;

        fileLock = fileLockDict.GetOrAdd(
            FilePath.ToUpperInvariant(),
            _ => new Object());
    }

    public void WriteBytes(byte[] bytes)
    {
        lock (fileLock)
        {
            CheckFiles();
            File.WriteAllBytes(TempPath, bytes);
            if (File.Exists(FilePath))
            {
                File.Move(FilePath, BackupPath, true);
            }
            File.Move(TempPath, FilePath);
        }
    }

    public void CheckFiles()
    {
        lock (fileLock)
        {
            if (File.Exists(LockPath))
            {
                throw new LockFileAlreadyExistsException(
                    $"Lock file {LockPath} already exists.");
            }
            if (File.Exists(TempPath))
            {
                throw new TempFileAlreadyExistsException(
                    $"Temporay file {TempPath} already exists.");
            }
            if (File.Exists(BackupPath) && !File.Exists(FilePath))
            {
                throw new BackupExistsWithoutMainException(
                    $"Backup file exists but could not find main file {FilePath}");
            }
        }
    }
}
