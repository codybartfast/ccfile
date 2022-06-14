
namespace Fmbm.IO.Tests;

public class RockValueTests
{
    readonly string filePath;
    readonly RockValue<Cake> rock;

    public RockValueTests()
    {
        filePath =
            Path.Combine(DirPaths.AppRoot.CheckedPath, "ValueFile.txt");
        rock = new RockValue<Cake>(filePath);
    }

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
    public void ValueFile_ReadWrite()
    {
        ClearFiles();
        var cake = new Cake();
        cake.Recipe = "ReadWrite";
        rock.Write(cake);
        Assert.Equal("ReadWrite", rock.Read()!.Recipe);
    }

    [Fact]
    public void ValueFile_Modify()
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
            cake!.Recipe = "ValueFile_Modify";
            return cake;
        });
        Assert.Equal("ValueFile_Modify", rock.Read()!.Recipe);
    }

    [Fact]
    public void ValueFile_ValueType()
    {
        ClearFiles();
        var valRock = new RockValue<int>(rock.FilePath);
        Assert.Equal(0, valRock.Read());
        valRock.Write(23);
        Assert.Equal(23, valRock.Read());
        valRock.Modify(_ => 42);
        Assert.Equal(42, valRock.Read());
    }

    [Fact]
    public void ValueFile_ArchveIsCalled()
    {
        ClearFiles();
        bool archiveCalled = false;
        RockFileArchive archive = (_, _1) =>
        {
            archiveCalled = true;
        };
        var newRock = new RockValue<Cake>(rock.FilePath, archive);
        Assert.False(archiveCalled);
        newRock.Write(new Cake());
        Assert.True(archiveCalled);
    }

    [Fact]
    public void ValueFile_ConstructorTakesRockFile()
    {
        ClearFiles();
        var rockFile = new RockFile(filePath);
        var rockVal = new RockValue<Cake>(rockFile);
        rockVal.Write(new Cake { Recipe = "Too many notes" });
        Assert.Equal("Too many notes", rockFile.ReadValue<Cake>()!.Recipe);
    }
}
