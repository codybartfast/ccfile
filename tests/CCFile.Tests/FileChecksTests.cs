namespace Fmbm.IO.Tests;

public class FileChecksTests
{
    CCFile ccfile = new CCFile(
        Path.Combine(DirPaths.AppRoot.CheckedPath, "FileChecks.txt"));

    void SetFiles(bool tmpExists, bool fileExists, bool bakExists)
    {
        SetFile(ccfile.TempPath, tmpExists);
        SetFile(ccfile.Path, fileExists);
        SetFile(ccfile.BackupPath, bakExists);
        void SetFile(string path, bool shouldExist)
        {
            if (shouldExist)
            {
                File.WriteAllText(path, string.Empty);
            }
            else
            {
                File.Delete(path);
            }
        }
    }

    [Fact]
    public void FileChecks_Exceptions()
    {
        var bools = new[] { false, true };
        var action = () => ccfile.CheckFiles();

        foreach (var tmpExists in bools)
        {
            foreach (var fileExists in bools)
            {
                foreach (var bakExists in bools)
                {
                    SetFiles(tmpExists, fileExists, bakExists);
                    if (tmpExists)
                    {
                        Assert.Throws<CCFileTempFileAlreadyExistsException>(action);
                    }
                    else if (bakExists)
                    {
                        if (!fileExists)
                        {
                            Assert.Throws<CCFileBackupExistsWithoutMainException>(action);
                        }
                    }
                }
            }
        }
    }
}