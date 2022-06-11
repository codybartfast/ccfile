namespace Fmbm.IO.Tests;

public class FileChecksTests
{
    RockFile rock = new RockFile(
        Path.Combine(DirPaths.AppRoot.CheckedPath, "FileChecks.txt"));

    void SetFiles(
        bool lckExists, bool tmpExists, bool fileExists, bool bakExists)
    {
        SetFile(rock.LockPath, lckExists);
        SetFile(rock.TempPath, tmpExists);
        SetFile(rock.FilePath, fileExists);
        SetFile(rock.BackupPath, bakExists);
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
        var action = () => rock.CheckFiles();

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