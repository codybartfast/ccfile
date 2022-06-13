
namespace Fmbm.IO;

using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Serializer = System.Text.Json.JsonSerializer;

public delegate void RockFileArchive(string filePath, string? backupPath);

public class RockFile :
    IRockFile, IRockBinary, IRockText, IRockGeneric
{
    const string LockSuffix = ".lck";
    const string BackupSuffix = ".bak";
    const string TempSuffix = ".tmp";

    static readonly Encoding encoding = Encoding.UTF8;

    static readonly ConcurrentDictionary<string, object> fileLockDict =
        new ConcurrentDictionary<string, object>();

    static readonly JsonSerializerOptions serializerOptions =
        new JsonSerializerOptions { WriteIndented = true, };

    public static TObject? BytesToObject<TObject>(byte[] bytes)
    {
        return TextToObject<TObject>(BytesToText(bytes));
    }

    public static TObject? TextToObject<TObject>(string text)
    {
        return Serializer.Deserialize<TObject>(text);
    }

    public static string BytesToText(byte[] bytes)
    {
        return encoding.GetString(bytes);
    }

    public static byte[] ObjectToBytes<TObject>(TObject obj)
    {
        return TextToBytes(ObjectToText<TObject>(obj));
    }

    public static string ObjectToText<TObject>(TObject obj)
    {
        return Serializer.Serialize<TObject>(obj, serializerOptions);
    }

    public static byte[] TextToBytes(string text)
    {
        return encoding.GetBytes(text);
    }

    static void NoOpArchive(string _, string? _1) { }

    private readonly object fileLock;

    public string FilePath { get; }
    internal string LockPath { get; }
    internal string BackupPath { get; }
    internal string TempPath { get; }

    readonly RockFileArchive archive;

    public RockFile(string filePath, RockFileArchive? archive = null)
    {
        FilePath = new FileInfo(filePath).FullName;
        LockPath = FilePath + LockSuffix;
        BackupPath = filePath + BackupSuffix;
        TempPath = filePath + TempSuffix;

        fileLock = fileLockDict.GetOrAdd(
            FilePath.ToUpperInvariant(),
            _ => new Object());

        this.archive = archive ?? NoOpArchive;
    }

    public void ModifyObject<TObject>(Func<TObject?, TObject> modify)
    {
        lock (fileLock)
        {
            TObject? modified = default;
            var existing = ReadObject<TObject>();
            try
            {
                modified = modify(existing);
            }
            catch (Exception modifyEx)
            {
                throw new RockFileModifyException(modifyEx);
            }
            WriteObject<TObject>(modified);
        }
    }

    public TObject? ReadObject<TObject>()
    {
        return BytesToObject<TObject>(ReadBytes());
    }

    public void WriteObject<TObject>(TObject obj)
    {
        WriteBytes(ObjectToBytes<TObject>(obj));
    }

    public void ModifyText(Func<string, string> modify)
    {
        lock (fileLock)
        {
            string? modified = null;
            var existing = ReadText();
            try
            {
                modified = modify(existing);
            }
            catch (Exception modifyEx)
            {
                throw new RockFileModifyException(modifyEx);
            }
            WriteText(modified);
        }
    }

    public string ReadText()
    {
        return BytesToText(ReadBytes());
    }

    public void WriteText(string text)
    {
        WriteBytes(TextToBytes(text));
    }

    public void ModifyBytes(Func<byte[], byte[]> modify)
    {
        lock (fileLock)
        {
            byte[]? modified = null;
            var existing = ReadBytes();
            try
            {
                modified = modify(existing);
            }
            catch (Exception modifyEx)
            {
                throw new RockFileModifyException(modifyEx);
            }
            WriteBytes(modified);
        }
    }

    public Byte[] ReadBytes()
    {
        lock (fileLock)
        {
            CheckFiles();
            return File.ReadAllBytes(FilePath);
        }
    }

    public void WriteBytes(byte[] bytes)
    {
        lock (fileLock)
        {
            CheckFiles();
            var foundExisting = File.Exists(FilePath);
            File.WriteAllBytes(TempPath, bytes);
            if (foundExisting)
            {
                File.Move(FilePath, BackupPath, true);
            }
            File.Move(TempPath, FilePath);
            try
            {
                archive(FilePath, foundExisting ? BackupPath : null);
            }
            catch (Exception archiveEx)
            {
                throw new RockFileArchiveException(archiveEx);
            }
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
