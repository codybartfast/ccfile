namespace Fmbm.IO.Tests;

public class FileDeleteTests
{
    CCFile ccfile = new CCFile(
        Path.Combine(DirPaths.AppRoot.CheckedPath, "FileDelete.txt"));



    [Fact]
    public void FileDelete_WhenNoFiles_DoesNotThrow()
    {
        File.Delete(ccfile.Path);
        File.Delete(ccfile.BackupPath);
        File.Delete(ccfile.TempPath);

        ccfile.Delete();
    }

    [Fact]
    public void WhenFilesExist_TheyAreDeleted()
    {
        File.WriteAllText(ccfile.Path, "Some Text");
        File.WriteAllText(ccfile.BackupPath, "Some Text");
        File.WriteAllText(ccfile.TempPath, "Some Text");

        ccfile.Delete();

        Assert.False(File.Exists(ccfile.Path));
        Assert.False(File.Exists(ccfile.BackupPath));
        Assert.False(File.Exists(ccfile.TempPath));
    }

    [Fact]
    public void GivenFilesExist_WhenValueDeleteCalled_FilesAreDeleted()
    {
        File.WriteAllText(ccfile.Path, "Some Text");
        File.WriteAllText(ccfile.BackupPath, "Some Text");
        File.WriteAllText(ccfile.TempPath, "Some Text");

        new CCValue<string>(ccfile.Path).Delete();

        Assert.False(File.Exists(ccfile.Path));
        Assert.False(File.Exists(ccfile.BackupPath));
        Assert.False(File.Exists(ccfile.TempPath));
    }
}