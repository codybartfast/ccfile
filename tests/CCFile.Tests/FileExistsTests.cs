using System.Collections.Generic;

namespace Fmbm.IO.Tests;

public class FileExistsTests
{
    CCFile ccfile = new CCFile(
        Path.Combine(DirPaths.AppRoot.CheckedPath, "FileExists.txt"));


    [Fact]
    public void FileExists()
    {
        var ccfile = new CCFile(
            Path.Combine(DirPaths.AppRoot.CheckedPath, "FileExists.txt"));
        ccfile.Delete();
        Assert.False(File.Exists(ccfile.Path));

        ccfile.WriteText("word");
        Assert.True(File.Exists(ccfile.Path));

        ccfile.Delete();
        Assert.False(File.Exists(ccfile.Path));
    }

    [Fact]
    public void ValueExists()
    {
        var ccvalue = new CCValue<List<string>>(
            Path.Combine(DirPaths.AppRoot.CheckedPath, "ValueExists.txt"));
        ccvalue.Delete();
        Assert.False(File.Exists(ccvalue.Path));

        ccvalue.Write(new List<string>());
        Assert.True(File.Exists(ccvalue.Path));

        ccvalue.Delete();
        Assert.False(File.Exists(ccvalue.Path));
    }

}