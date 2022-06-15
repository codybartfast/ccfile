
namespace Fmbm.IO.Tests;

public class CCValueTests
{
    readonly string filePath;
    readonly CCValue<Cake> ccfile;

    public CCValueTests()
    {
        filePath =
            Path.Combine(DirPaths.AppRoot.CheckedPath, "ValueFile.txt");
        ccfile = new CCValue<Cake>(filePath);
    }

    void ClearFiles()
    {
        File.Delete(ccfile.CCFile.LockPath);
        File.Delete(ccfile.CCFile.TempPath);
        File.Delete(ccfile.CCFile.Path);
        File.Delete(ccfile.CCFile.BackupPath);
    }

    [Fact]
    public void ReadValue_NoFile_ReturnsDefault()
    {
        ClearFiles();
        Assert.Null(ccfile.Read());
    }

    [Fact]
    public void ValueFile_ReadWrite()
    {
        ClearFiles();
        var cake = new Cake();
        cake.Recipe = "ReadWrite";
        ccfile.Write(cake);
        Assert.Equal("ReadWrite", ccfile.Read()!.Recipe);
    }

    [Fact]
    public void ValueFile_Modify()
    {
        ClearFiles();
        ccfile.Modify(cake =>
        {
            Assert.Null(cake);
            return new Cake();
        });
        ccfile.Modify(cake =>
        {
            Assert.NotNull(cake);
            cake!.Recipe = "ValueFile_Modify";
            return cake;
        });
        Assert.Equal("ValueFile_Modify", ccfile.Read()!.Recipe);
    }

    [Fact]
    public void ValueFile_ValueType()
    {
        ClearFiles();
        var valCC = new CCValue<int>(ccfile.Path);
        Assert.Equal(0, valCC.Read());
        valCC.Write(23);
        Assert.Equal(23, valCC.Read());
        valCC.Modify(_ => 42);
        Assert.Equal(42, valCC.Read());
    }

    [Fact]
    public void ValueFile_ArchveIsCalled()
    {
        ClearFiles();
        bool archiveCalled = false;
        Action<string, string?> archive = (_, _1) =>
        {
            archiveCalled = true;
        };
        var newCC = new CCValue<Cake>(ccfile.Path, archive);
        Assert.False(archiveCalled);
        newCC.Write(new Cake());
        Assert.True(archiveCalled);
    }

    public void ReadOrWritet_ReturnsExisting()
    {
        ClearFiles();
        var saved = new Cake { Recipe = "He didn't say much" };
        var created = new Cake { Recipe = "Poling box 246" };
        Func<Cake> getValue = () => created;
        ccfile.Write(saved);
        var actual = ccfile.ReadOrWrite(getValue);
        Assert.Equal(saved.Recipe, actual.Recipe);
    }

    [Fact]
    public void ReadOrWriteObject_ReturnsCreated()
    {
        ClearFiles();
        // var saved = new Cake{Recipe = "stay the same};
        var created = new Cake { Recipe = "A wee big scary" };
        Func<Cake> getValue = () => created;
        // ccfile.WriteValue(saved);
        var actual = ccfile.ReadOrWrite(getValue);
        Assert.Equal(created.Recipe, actual.Recipe);
    }

    [Fact]
    public void CCValue_ThreeArgConstructor_Existing()
    {
        ClearFiles();
        ccfile.Write(new Cake { Recipe = "Big Eyes" });
        var archiveCalled = false;
        var three = new CCValue<Cake>(
            ccfile.Path,
            () => new Cake { Recipe = "Referendum" },
            (_, _1) => { archiveCalled = true; });
        Assert.False(archiveCalled);
        Assert.Equal("Big Eyes", three.Read()!.Recipe);
    }

    [Fact]
    public void CCValue_ThreeArgConstructor_Construct()
    {
        ClearFiles();
        // ccfile.Write(new Cake { Recipe = "Big Eyes" });
        var archiveCalled = false;
        var three = new CCValue<Cake>(
            ccfile.Path,
            () => new Cake { Recipe = "Referendum" },
            (_, _1) => { archiveCalled = true; });
        Assert.True(archiveCalled);
        Assert.Equal("Referendum", three.Read()!.Recipe);
    }

    // [Fact]
    // public void ValueFile_ConstructorTakesCCFile()
    // {
    //     ClearFiles();
    //     var ccFile = new CCFile(filePath);
    //     var ccVal = new CCValue<Cake>(ccFile);
    //     ccVal.Write(new Cake { Recipe = "Too many notes" });
    //     Assert.Equal("Too many notes", ccFile.ReadValue<Cake>()!.Recipe);
    // }
}
