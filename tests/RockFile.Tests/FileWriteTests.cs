using System.Text;

namespace Fmbm.IO.Tests;

public class FileWriteTests
{

    readonly Encoding encoding = Encoding.UTF8;

    readonly RockFile rock;

    public FileWriteTests()
    {
        rock = new RockFile(
            Path.Combine(DirPaths.AppRoot.CheckedPath, "FileWrite.txt"));
        File.Delete(rock.LockPath);
        File.Delete(rock.TempPath);
        File.Delete(rock.FilePath);
        File.Delete(rock.BackupPath);
    }

    [Fact]
    public void WriteBytes_WriteToDisk()
    {
        var text = DateTime.UtcNow.ToString("o") + "WritesToDisk";
        var bytes = encoding.GetBytes(text);
        rock.WriteBytes(bytes);
        var diskText = encoding.GetString(File.ReadAllBytes(rock.FilePath));
        Assert.Equal(text, diskText);
    }

}

