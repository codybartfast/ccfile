﻿
namespace Fmbm.IO;

using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Serializer = System.Text.Json.JsonSerializer;

public class CCFile :
    ICCFile, ICCBinary, ICCText, ICCGeneric
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

    public string Path { get; }
    internal string LockPath { get; }
    internal string BackupPath { get; }
    internal string TempPath { get; }

    readonly Action<string, string?> archive;

    public CCFile(string filePath, Action<string, string?>? archive = null)
    {
        Path = new FileInfo(filePath).FullName;
        LockPath = Path + LockSuffix;
        BackupPath = filePath + BackupSuffix;
        TempPath = filePath + TempSuffix;

        fileLock = fileLockDict.GetOrAdd(
            Path.ToUpperInvariant(),
            _ => new Object());

        this.archive = archive ?? NoOpArchive;
    }

    // public CCValue<TValue> CreateValueFile<TValue>()
    // {
    //     return new CCValue<TValue>(this);
    // }

    public TValue ReadOrWriteValue<TValue>(Func<TValue> getInitalValue)
    {
        lock (fileLock)
        {
            if (!File.Exists(Path))
            {
                TValue value;
                try
                {
                    value = getInitalValue();
                }
                catch (Exception getValueEx)
                {
                    throw new CCFileGetInitialValueException(getValueEx);
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
                throw new CCFileModifyException(modifyEx);
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
            if (!File.Exists(Path))
            {
                string value;
                try
                {
                    value = getInitialValue();
                }
                catch (Exception getValueEx)
                {
                    throw new CCFileGetInitialValueException(getValueEx);
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
                throw new CCFileModifyException(modifyEx);
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
            if (!File.Exists(Path))
            {
                byte[] value;
                try
                {
                    value = getInitialValue();
                }
                catch (Exception getValueEx)
                {
                    throw new
                        CCFileGetInitialValueException(getValueEx);
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
                throw new CCFileModifyException(modifyEx);
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
            return File.Exists(Path) ? File.ReadAllBytes(Path) : null;
        }
    }

    public void WriteBytes(byte[] bytes)
    {
        lock (fileLock)
        {
            CheckFiles();
            var foundExisting = File.Exists(Path);
            File.WriteAllBytes(TempPath, bytes);
            if (foundExisting)
            {
                File.Move(Path, BackupPath, true);
            }
            File.Move(TempPath, Path);
            try
            {
                archive(Path, foundExisting ? BackupPath : null);
            }
            catch (Exception archiveEx)
            {
                throw new CCFileArchiveException(archiveEx);
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
            if (File.Exists(BackupPath) && !File.Exists(Path))
            {
                throw new BackupExistsWithoutMainException(
                    $"Backup file exists but could not find main file {Path}");
            }
        }
    }
}