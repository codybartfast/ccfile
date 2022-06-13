
namespace Fmbm.IO.Tests;

public class RockFileObjectTests
{
    RockFileObject<Cake> rock = new RockFileObject<Cake>(
        Path.Combine(DirPaths.AppRoot.CheckedPath, "FileObject.txt"));

    void ClearFiles()
    {
        File.Delete(rock.RockFile.LockPath);
        File.Delete(rock.RockFile.TempPath);
        File.Delete(rock.RockFile.FilePath);
        File.Delete(rock.RockFile.BackupPath);
    }

    [Fact]
    public void ReadObject_NoFile_ReturnsDefault()
    {
        ClearFiles();
        Assert.Null(rock.Read());
    }

    [Fact]
    public void FileObject_ReadWrite()
    {
        ClearFiles();
        var cake = new Cake();
        cake.Recipe = "ReadWrite";
        rock.Write(cake);
        Assert.Equal("ReadWrite", rock.Read()!.Recipe);
    }

    [Fact]
    public void FileObject_Modify()
    {
        ClearFiles();
        rock.Modify(cake =>
        {
            Assert.Null(cake);
            return new Cake();
        });
        rock.Modify(cake =>
        {
            Assert.NotNull(cake);
            cake!.Recipe = "FileObject_Modify";
            return cake;
        });
        Assert.Equal("FileObject_Modify", rock.Read()!.Recipe);
    }

    [Fact]
    public void FileObject_ValueType()
    {
        ClearFiles();
        var valRock = new RockFileObject<int>(rock.FilePath);
        Assert.Equal(0, valRock.Read());
        valRock.Write(23);
        Assert.Equal(23, valRock.Read());
        valRock.Modify(_ => 42);
        Assert.Equal(42, valRock.Read());
    }
}