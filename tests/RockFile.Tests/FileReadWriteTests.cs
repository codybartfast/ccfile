using System.Text;

namespace Fmbm.IO.Tests;

public class FileReadWriteTests
{

    readonly Encoding encoding = Encoding.UTF8;

    readonly RockFile rock;

    public FileReadWriteTests()
    {
        rock = new RockFile(
            Path.Combine(DirPaths.AppRoot.CheckedPath, "FileWrite.txt"));
    }

    void ClearFiles()
    {
        File.Delete(rock.LockPath);
        File.Delete(rock.TempPath);
        File.Delete(rock.FilePath);
        File.Delete(rock.BackupPath);
    }

    [Fact]
    public void WriteBytes_WriteToDisk()
    {
        ClearFiles();
        var text = DateTime.UtcNow.ToString("o") + "WritesToDisk";
        var bytes = encoding.GetBytes(text);
        rock.WriteBytes(bytes);
        var diskBytes = File.ReadAllBytes(rock.FilePath);
        Assert.Equal(bytes, diskBytes);
    }

    [Fact]
    public void ReadBytes_ReadsFromDisk()
    {
        ClearFiles();
        var text = DateTime.UtcNow.ToString("o") + "ReadsFromDisk";
        var bytes = encoding.GetBytes(text);
        File.WriteAllBytes(rock.FilePath, bytes);
        var rockBytes = rock.ReadBytes();
        Assert.Equal(bytes, rockBytes);        
    }
}

