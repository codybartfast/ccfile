
namespace Fmbm.IO.Tests;

public class RockFileValueTests
{
    RockValueFile<Cake> rock = new RockValueFile<Cake>(
        Path.Combine(DirPaths.AppRoot.CheckedPath, "FileValue.txt"));

    void ClearFiles()
    {
        File.Delete(rock.RockFile.LockPath);
        File.Delete(rock.RockFile.TempPath);
        File.Delete(rock.RockFile.FilePath);
        File.Delete(rock.RockFile.BackupPath);
    }

    [Fact]
    public void ReadValue_NoFile_ReturnsDefault()
    {
        ClearFiles();
        Assert.Null(rock.Read());
    }

    [Fact]
    public void FileValue_ReadWrite()
    {
        ClearFiles();
        var cake = new Cake();
        cake.Recipe = "ReadWrite";
        rock.Write(cake);
        Assert.Equal("ReadWrite", rock.Read()!.Recipe);
    }

    [Fact]
    public void FileValue_Modify()
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
            cake!.Recipe = "FileValue_Modify";
            return cake;
        });
        Assert.Equal("FileValue_Modify", rock.Read()!.Recipe);
    }

    [Fact]
    public void FileValue_ValueType()
    {
        ClearFiles();
        var valRock = new RockValueFile<int>(rock.FilePath);
        Assert.Equal(0, valRock.Read());
        valRock.Write(23);
        Assert.Equal(23, valRock.Read());
        valRock.Modify(_ => 42);
        Assert.Equal(42, valRock.Read());
    }

    [Fact]
    public void ValueFile_ArchveIsCalled(){
        ClearFiles();
        bool archiveCalled = false;
        RockFileArchive archive = (_, _1) => {
            archiveCalled = true;
        };
        var newRock = new RockValueFile<Cake>(rock.FilePath, archive);
        Assert.False(archiveCalled);
        newRock.Write(new Cake());
        Assert.True(archiveCalled);
    }
}