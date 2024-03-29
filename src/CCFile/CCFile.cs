﻿
namespace Fmbm.IO;

using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Serializer = System.Text.Json.JsonSerializer;

public class CCFile : ICCFile
{
    const string BackupSuffix = ".bak";
    const string TempSuffix = ".tmp";

    static readonly Encoding encoding = Encoding.UTF8;

    static readonly ConcurrentDictionary<string, object> fileLockDict =
        new ConcurrentDictionary<string, object>();

    static readonly JsonSerializerOptions serializerOptions =
        new JsonSerializerOptions { WriteIndented = true };

    public static TValue BytesToValue<TValue>(byte[] bytes)
    {
        var stream = new MemoryStream(bytes);
        return Serializer.Deserialize<TValue>(stream)!;
    }

    public static TValue TextToValue<TValue>(string text)
    {
        return Serializer.Deserialize<TValue>(text)!;
    }

    public static string BytesToText(byte[] bytes)
    {
        return encoding.GetString(bytes);
    }

    public static byte[] ValueToBytes<TValue>(TValue value)
    {
        var stream = new MemoryStream();
        Serializer.Serialize<TValue>(stream, value, serializerOptions);
        return stream.ToArray();
    }

    public static string ValueToText<TValue>(TValue value)
    {
        return Serializer.Serialize<TValue>(value, serializerOptions);
    }

    public static byte[] TextToBytes(string text)
    {
        return encoding.GetBytes(text);
    }

    static void NoOpArchive(string _, string? _1) { }

    private readonly object fileLock;

    public string Path { get; }
    internal string BackupPath { get; }
    internal string TempPath { get; }

    readonly Action<string, string?> archive;

    public CCFile(string filePath, Action<string, string?>? archive = null)
    {
        Path = new FileInfo(filePath).FullName;
        BackupPath = filePath + BackupSuffix;
        TempPath = filePath + TempSuffix;

        fileLock = fileLockDict.GetOrAdd(
            Path.ToUpperInvariant(),
            _ => new Object());

        this.archive = archive ?? NoOpArchive;
    }

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

    public TValue ModifyValue<TValue>(Func<TValue, TValue> modify)
    {
        lock (fileLock)
        {
            TValue modified;
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

    public TValue ReadValue<TValue>()
    {
        byte[] bytes = ReadBytes();
        var val = BytesToValue<TValue>(bytes);
        return val ?? throw new CCFileNullDeserializeResultException(Path);
    }

    public void WriteValue<TValue>(TValue value)
    {
        WriteBytes(ValueToBytes<TValue>(value));
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

    public string ModifyText(Func<string, string> modify)
    {
        lock (fileLock)
        {
            string modified;
            string existing = ReadText();
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

    public string ReadText()
    {
        byte[] bytes = ReadBytes();
        return BytesToText(bytes);
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
                    throw new CCFileGetInitialValueException(getValueEx);
                }
                WriteBytes(value);
            }
            return ReadBytes();
        }
    }

    public byte[] ModifyBytes(Func<byte[], byte[]> modify)
    {
        lock (fileLock)
        {
            byte[] modified;
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

    public Byte[] ReadBytes()
    {
        lock (fileLock)
        {
            CheckFiles();
            return File.ReadAllBytes(Path);
        }
    }

    public void WriteBytes(byte[] bytes)
    {
        lock (fileLock)
        {
            CheckFiles();
            CheckAccess(BackupPath);
            CheckAccess(Path);

            using (var stream = File.Open(TempPath,
                FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            var makeBackup = File.Exists(Path);
            if (makeBackup)
            {
                File.Move(Path, BackupPath, true);
            }
            File.Move(TempPath, Path);
            try
            {
                archive(Path, makeBackup ? BackupPath : null);
            }
            catch (Exception archiveEx)
            {
                throw new CCFileArchiveException(archiveEx);
            }
        }
    }

    public bool Exists => File.Exists(Path);

    public void Delete()
    {
        lock (fileLock)
        {
            File.Delete(Path);
            File.Delete(BackupPath);
            File.Delete(TempPath);
        }
    }

    public void CheckFiles()
    {
        lock (fileLock)
        {
            if (File.Exists(TempPath))
            {
                throw new CCFileTempFileAlreadyExistsException(
                    $"Temporay file {TempPath} already exists.");
            }
            if (File.Exists(BackupPath) && !File.Exists(Path))
            {
                throw new CCFileBackupExistsWithoutMainException(
                    $"Backup file exists but could not find main file {Path}");
            }
        }
    }

    private void CheckAccess(string filePahth)
    {
        if (File.Exists(filePahth))
        {
            using (File.Open(filePahth,
                FileMode.Open, FileAccess.Read, FileShare.None))
            {
                // Opening file just to check that we have access.
            }
        }
    }
}
