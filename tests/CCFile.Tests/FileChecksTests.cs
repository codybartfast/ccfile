namespace Fmbm.IO.Tests;

public class FileChecksTests
{
    CCFile ccfile = new CCFile(
        Path.Combine(DirPaths.AppRoot.CheckedPath, "FileChecks.txt"));

    void SetFiles(
        bool lckExists, bool tmpExists, bool fileExists, bool bakExists)
    {
        SetFile(ccfile.LockPath, lckExists);
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

        foreach (var lckExists in bools)
        {
            foreach (var tmpExists in bools)
            {
                foreach (var fileExists in bools)
                {
                    foreach (var bakExists in bools)
                    {
                        SetFiles(lckExists, tmpExists, fileExists, bakExists);
                        if (lckExists)
                        {
                            Assert.Throws<LockFileAlreadyExistsException>(action);
                        }
                        else if (tmpExists)
                        {
                            Assert.Throws<TempFileAlreadyExistsException>(action);
                        }
                        else if (bakExists)
                        {
                            if (!fileExists)
                            {
                                Assert.Throws<BackupExistsWithoutMainException>(action);
                            }
                        }
                    }
                }
            }
        }
    }
}