namespace Fmbm.IO.Tests;

public class FilePathTests
{
    string tempDir = DirPaths.GetDir("FilePath_Test").CheckedPath;

    [Fact]
    public void FilePath_NotRelative()
    {
        // FilePath shouldn't be stored as a relative path or its location
        // will be dependant on the Current directory.

        var filePath = "Apple";
        var rock = new RockFile(filePath);
        var fullName1 = new FileInfo(rock.FilePath).FullName;
        var cwd = Environment.CurrentDirectory;
        Environment.CurrentDirectory = tempDir;
        var fullName2 = new FileInfo(rock.FilePath).FullName;
        Environment.CurrentDirectory = cwd;
        Assert.Equal(fullName1, fullName2);
    }

    [Fact]
    public void FilePath_SupportFilePaths(){
        var dir = tempDir;
        var fileName = "Some.File.txt";
        var lockName = "Some.File.txt.lck";
        var backupName = "Some.File.txt.bak";
        var newName = "Some.File.txt.tmp";
        var filePath = Path.Combine(dir, fileName);
        var rock = new RockFile(filePath);

        Assert.Equal(dir, new FileInfo(rock.LockPath).Directory?.FullName);
        Assert.Equal(lockName, new FileInfo(rock.LockPath).Name);

        Assert.Equal(dir, new FileInfo(rock.BackupPath).Directory?.FullName);
        Assert.Equal(backupName, new FileInfo(rock.BackupPath).Name);

        Assert.Equal(dir, new FileInfo(rock.NewPath).Directory?.FullName);
        Assert.Equal(newName, new FileInfo(rock.NewPath).Name);
    }
}