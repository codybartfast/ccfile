
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
        File.Delete(rock.RockFile.Path);
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
        var valRock = new RockValue<int>(rock.Path);
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
        var newRock = new RockValue<Cake>(rock.Path, archive);
        Assert.False(archiveCalled);
        newRock.Write(new Cake());
        Assert.True(archiveCalled);
    }

    public void ReadOrWritet_ReturnsExisting()
    {
        ClearFiles();
        var saved = new Cake { Recipe = "He didn't say much" };
        var created = new Cake { Recipe = "Poling box 246" };
        Func<Cake> getValue = () => created;
        rock.Write(saved);
        var actual = rock.ReadOrWrite(getValue);
        Assert.Equal(saved.Recipe, actual.Recipe);
    }

    [Fact]
    public void ReadOrWriteObject_ReturnsCreated()
    {
        ClearFiles();
        // var saved = new Cake{Recipe = "stay the same};
        var created = new Cake { Recipe = "A wee big scary" };
        Func<Cake> getValue = () => created;
        // rock.WriteValue(saved);
        var actual = rock.ReadOrWrite(getValue);
        Assert.Equal(created.Recipe, actual.Recipe);
    }

    [Fact]
    public void RockValue_ThreeArgConstructor_Existing()
    {
        ClearFiles();
        rock.Write(new Cake { Recipe = "Big Eyes" });
        var archiveCalled = false;
        var three = new RockValue<Cake>(
            rock.Path,
            () => new Cake { Recipe = "Referendum" },
            (_, _1) => { archiveCalled = true; });
        Assert.False(archiveCalled);
        Assert.Equal("Big Eyes", three.Read()!.Recipe);
    }

    [Fact]
    public void RockValue_ThreeArgConstructor_Construct()
    {
        ClearFiles();
        // rock.Write(new Cake { Recipe = "Big Eyes" });
        var archiveCalled = false;
        var three = new RockValue<Cake>(
            rock.Path,
            () => new Cake { Recipe = "Referendum" },
            (_, _1) => { archiveCalled = true; });
        Assert.True(archiveCalled);
        Assert.Equal("Referendum", three.Read()!.Recipe);
    }

    // [Fact]
    // public void ValueFile_ConstructorTakesRockFile()
    // {
    //     ClearFiles();
    //     var rockFile = new RockFile(filePath);
    //     var rockVal = new RockValue<Cake>(rockFile);
    //     rockVal.Write(new Cake { Recipe = "Too many notes" });
    //     Assert.Equal("Too many notes", rockFile.ReadValue<Cake>()!.Recipe);
    // }
}
