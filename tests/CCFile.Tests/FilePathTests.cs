namespace Fmbm.IO.Tests;

public class FilePathTests
{
    string tempDir = DirPaths.GetDir("FilePathTests").CheckedPath;

    [Fact]
    public void FilePath_NotRelative()
    {
        // FilePath shouldn't be stored as a relative path or its location
        // will be dependant on the Current directory.

        var filePath = "Apple";
        var ccfile = new CCFile(filePath);
        var fullName1 = new FileInfo(ccfile.Path).FullName;
        var cwd = Environment.CurrentDirectory;
        Environment.CurrentDirectory = tempDir;
        var fullName2 = new FileInfo(ccfile.Path).FullName;
        Environment.CurrentDirectory = cwd;
        Assert.Equal(fullName1, fullName2);
    }

    [Fact]
    public void FilePath_FullNames(){
        var dir = tempDir;
        var fileName = "Some.File.txt";
        var backupName = "Some.File.txt.bak";
        var newName = "Some.File.txt.tmp";
        var filePath = Path.Combine(dir, fileName);
        var ccfile = new CCFile(filePath);

        Assert.Equal(dir, new FileInfo(ccfile.BackupPath).Directory?.FullName);
        Assert.Equal(backupName, new FileInfo(ccfile.BackupPath).Name);

        Assert.Equal(dir, new FileInfo(ccfile.TempPath).Directory?.FullName);
        Assert.Equal(newName, new FileInfo(ccfile.TempPath).Name);
    }
}