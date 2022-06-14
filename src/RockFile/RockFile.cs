
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

    public static TValue? BytesToValue<TValue>(byte[] bytes)
    {
        return TextToValue<TValue>(BytesToText(bytes));
    }

    public static TValue? TextToValue<TValue>(string text)
    {
        return Serializer.Deserialize<TValue>(text);
    }

    public static string BytesToText(byte[] bytes)
    {
        return encoding.GetString(bytes);
    }

    public static byte[] ValueToBytes<TValue>(TValue obj)
    {
        return TextToBytes(ValueToText<TValue>(obj));
    }

    public static string ValueToText<TValue>(TValue obj)
    {
        return Serializer.Serialize<TValue>(obj, serializerOptions);
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

    // public RockValue<TValue> CreateValueFile<TValue>()
    // {
    //     return new RockValue<TValue>(this);
    // }

    public TValue ReadOrWriteValue<TValue>(Func<TValue> getInitalValue)
    {
        lock (fileLock)
        {
            if (!File.Exists(FilePath))
            {
                TValue value;
                try
                {
                    value = getInitalValue();
                }
                catch (Exception getValueEx)
                {
                    throw new RockFileGetInitialValueException(getValueEx);
                }
                WriteValue(value);
            }
            return ReadValue<TValue>()!;
        }
    }

    public TValue ModifyValue<TValue>(Func<TValue?, TValue> modify)
    {
        lock (fileLock)
        {
            TValue? modified = default;
            var existing = ReadValue<TValue>();
            try
            {
                modified = modify(existing);
            }
            catch (Exception modifyEx)
            {
                throw new RockFileModifyException(modifyEx);
            }
            WriteValue<TValue>(modified);
            return modified;
        }
    }

    public TValue? ReadValue<TValue>()
    {
        byte[]? bytes = ReadBytes();
        return bytes is null ? default : BytesToValue<TValue>(bytes);
    }

    public void WriteValue<TValue>(TValue obj)
    {
        WriteBytes(ValueToBytes<TValue>(obj));
    }

    public string ReadOrWriteText(Func<string> getInitialValue)
    {
        lock (fileLock)
        {
            if (!File.Exists(FilePath))
            {
                string value;
                try
                {
                    value = getInitialValue();
                }
                catch (Exception getValueEx)
                {
                    throw new RockFileGetInitialValueException(getValueEx);
                }
                WriteText(value);
            }
            return ReadText()!;
        }

    }

    public string ModifyText(Func<string?, string> modify)
    {
        lock (fileLock)
        {
            string? modified = null;
            string? existing = ReadText();
            try
            {
                modified = modify(existing);
            }
            catch (Exception modifyEx)
            {
                throw new RockFileModifyException(modifyEx);
            }
            WriteText(modified);
            return modified;
        }
    }

    public string? ReadText()
    {
        byte[]? bytes = ReadBytes();
        return bytes is null ? null : BytesToText(bytes);
    }

    public void WriteText(string text)
    {
        WriteBytes(TextToBytes(text));
    }

    public byte[] ReadOrWriteBytes(Func<Byte[]> getInitialValue)
    {
        lock (fileLock)
        {
            if (!File.Exists(FilePath))
            {
                byte[] value;
                try
                {
                    value = getInitialValue();
                }
                catch (Exception getValueEx)
                {
                    throw new
                        RockFileGetInitialValueException(getValueEx);
                }
                WriteBytes(value);
            }
            return ReadBytes()!;
        }
    }

    public byte[] ModifyBytes(Func<byte[]?, byte[]> modify)
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
            return modified;
        }
    }

    public Byte[]? ReadBytes()
    {
        lock (fileLock)
        {
            CheckFiles();
            return File.Exists(FilePath) ? File.ReadAllBytes(FilePath) : null;
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
