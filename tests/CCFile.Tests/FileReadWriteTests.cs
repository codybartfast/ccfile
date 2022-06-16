using System.Text;
using System.Threading.Tasks;

namespace Fmbm.IO.Tests;

public class FileReadWriteTests
{
    readonly Encoding encoding = Encoding.UTF8;

    readonly CCFile ccfile;

    public FileReadWriteTests()
    {
        ccfile = new CCFile(
            Path.Combine(DirPaths.AppRoot.CheckedPath, "FileReadWrite.txt"));
    }

    void ClearFiles()
    {
        File.Delete(ccfile.LockPath);
        File.Delete(ccfile.TempPath);
        File.Delete(ccfile.Path);
        File.Delete(ccfile.BackupPath);
    }

    [Fact]
    public void NoFile_ReturnsDefault()
    {
        ClearFiles();
        Assert.Null(ccfile.ReadBytes());
        Assert.Null(ccfile.ReadText());
        Assert.Null(ccfile.ReadValue<Cake>());
        Assert.Equal(0, ccfile.ReadValue<int>());
    }

    [Fact]
    public void WriteBytes_WriteToDisk()
    {
        ClearFiles();
        var text = DateTime.UtcNow.ToString("o") + "WritesToDisk";
        var bytes = encoding.GetBytes(text);
        ccfile.WriteBytes(bytes);
        var diskBytes = File.ReadAllBytes(ccfile.Path);
        Assert.Equal(bytes, diskBytes);
    }

    [Fact]
    public void ReadBytes_ReadsFromDisk()
    {
        ClearFiles();
        var text = DateTime.UtcNow.ToString("o") + "ReadsFromDisk";
        var bytes = encoding.GetBytes(text);
        File.WriteAllBytes(ccfile.Path, bytes);
        var ccBytes = ccfile.ReadBytes();
        Assert.Equal(bytes, ccBytes);
    }

    [Fact]
    public void Text_IsWrittenAndReadFromDisk()
    {
        ClearFiles();
        ccfile.WriteText("Apple Banana Cherry");
        var diskText = File.ReadAllText(ccfile.Path);
        File.WriteAllText(ccfile.Path,
            diskText.Replace("Banana", "Blueberry"));
        Assert.Equal("Apple Blueberry Cherry", ccfile.ReadText());
    }

    [Fact]
    public void Value_ReferencesNotEquel()
    {
        ClearFiles();
        var cake = new Cake();
        ccfile.WriteValue<Cake>(cake);
        var cake2 = ccfile.ReadValue<Cake>()!;
        Assert.NotSame(cake, cake2);
        Assert.Equal(cake.HasRaisins, cake2.HasRaisins);
        Assert.Equal(cake.SliceCount, cake2.SliceCount);
        Assert.Equal(cake.Recipe, cake2.Recipe);
    }

    [Fact]
    public void Value_IsWrittenAndReadFromDisk()
    {
        ClearFiles();
        var cake = new Cake();
        ccfile.WriteValue<Cake>(cake);
        var diskText = File.ReadAllText(ccfile.Path);
        File.WriteAllText(ccfile.Path, diskText.Replace("Stir", "Mix"));
        var cake2 = ccfile.ReadValue<Cake>()!;
        Assert.Equal(true, cake2.HasRaisins);
        Assert.Equal(6, cake2.SliceCount);
        Assert.NotEqual(cake.Recipe, cake2.Recipe);
        Assert.Equal("Mix loads", cake2.Recipe);
    }

    [Fact]
    public void File_IsBackedUp()
    {
        ClearFiles();
        ccfile.WriteText("Apple");
        Assert.Equal("Apple", File.ReadAllText(ccfile.Path));
        Assert.False(File.Exists(ccfile.BackupPath));

        ccfile.WriteText("Banana");
        Assert.Equal("Banana", File.ReadAllText(ccfile.Path));
        Assert.Equal("Apple", File.ReadAllText(ccfile.BackupPath));

        ccfile.WriteText("Cherry");
        Assert.Equal("Cherry", File.ReadAllText(ccfile.Path));
        Assert.Equal("Banana", File.ReadAllText(ccfile.BackupPath));
    }

    [Fact]
    public void Value_Modify_IsCalled()
    {
        var newRecipe = "Bake well";
        var cake = new Cake();
        Assert.NotEqual(newRecipe, cake.Recipe);
        ccfile.WriteValue<Cake>(cake);
        ccfile.ModifyValue<Cake>(cake =>
        {
            cake!.SliceCount = 177;
            cake.Recipe = newRecipe;
            return cake;
        });
        var readCake = ccfile.ReadValue<Cake>()!;
        Assert.Equal(newRecipe, readCake.Recipe);
        Assert.Equal(177, readCake.SliceCount);
    }

    [Fact]
    public void Text_Modify_IsCalled()
    {
        ClearFiles();
        ccfile.WriteText("One Two Three");
        ccfile.ModifyText(text => text!.Replace("Two", "And"));
        Assert.Equal("One And Three", ccfile.ReadText());
    }

    [Fact]
    public void Binary_Modify_IsCalled()
    {
        ClearFiles();
        ccfile.WriteBytes(encoding.GetBytes("Cat"));
        ccfile.ModifyBytes(bytes =>
        {
            bytes![0] = 66;
            return bytes;
        });
        Assert.Equal("Bat", encoding.GetString(ccfile.ReadBytes()!));
    }

    [Fact]
    public void AfterWrite_ArchiveIsCalled()
    {
        string? fileContent = null;
        string? backupContent = null;

        ClearFiles();
        var arcCC = new CCFile(this.ccfile.Path, Archive);

        arcCC.WriteText("Apple Pie");
        Assert.Equal(fileContent, "Apple Pie");
        Assert.Null(backupContent);

        arcCC.WriteText("Banana Split");
        Assert.Equal(fileContent, "Banana Split");
        Assert.Equal(backupContent, "Apple Pie");

        arcCC.WriteText("Cherry Cola");
        Assert.Equal(fileContent, "Cherry Cola");
        Assert.Equal(backupContent, "Banana Split");

        void Archive(string filePath, string? backupPath)
        {
            fileContent = File.ReadAllText(filePath);
            if (backupPath is not null)
            {
                backupContent = File.ReadAllText(backupPath);
                File.Delete(backupPath);
            }
        }
    }

    [Fact]
    public void MultipleInstances_Writing_ShareSameLock()
    {
        ClearFiles();
        var blockTask1 = true;
        Func<string?, string> modify = txt =>
        {
            while (blockTask1)
            {
                Task.Delay(10).Wait();
            }
            return "America";
        };
        var ccfile1 = new CCFile(ccfile.Path.ToLower());
        var ccfile2 = new CCFile(ccfile.Path.ToUpper());
        var task1 = new Task(() => ccfile1.ModifyText(modify));

        task1.Start();
        Task.Delay(100).Wait(); // time for task1 to attain lock
        var task2 = new Task(() => ccfile2.WriteText("Brazil"));
        task2.Start();
        Task.Delay(500).Wait(); // time for task2 to finish if not blocked
        Assert.False(task1.IsCompleted);
        Assert.False(task2.IsCompleted);

        blockTask1 = false;
        Task.Delay(100).Wait();
        Assert.True(task1.IsCompleted);
        Assert.True(task2.IsCompleted);
        Assert.Equal("Brazil", ccfile1.ReadText());
        Assert.Equal("Brazil", ccfile2.ReadText());
        Assert.Equal("America", File.ReadAllText(ccfile.BackupPath));
    }

    [Fact]
    public void ReadOrWriteBytes_ReturnsExisting()
    {
        ClearFiles();
        var saved = new byte[] { 3, 1, 4, 1, 5 };
        var created = new byte[] { 2, 7, 1, 8, 2 };
        Func<byte[]> getValue = () => created;
        ccfile.WriteBytes(saved);
        var actual = ccfile.ReadOrWriteBytes(getValue);
        Assert.Equal(saved, actual);
    }

    [Fact]
    public void ReadOrWriteBytes_WritesWhenNull()
    {
        ClearFiles();
        // var saved = new byte[] { 3, 1, 4, 1, 5 };
        var created = new byte[] { 2, 7, 1, 8, 2 };
        Func<byte[]> getValue = () => created;
        // ccfile.WriteBytes(saved);
        var actual = ccfile.ReadOrWriteBytes(getValue);
        Assert.Equal(created, actual);
    }

    [Fact]
    public void ReadOrWriteString_ReturnsExisting()
    {
        ClearFiles();
        var saved = "storage rocks";
        var created = "get new strings!";
        Func<string> getValue = () => created;
        ccfile.WriteText(saved);
        var actual = ccfile.ReadOrWriteText(getValue);
        Assert.Equal(saved, actual);
    }

    [Fact]
    public void ReadOrWriteString_ReturnsCreated()
    {
        ClearFiles();
        // var saved = "storage rocks";
        var created = "get new strings!";
        Func<string> getValue = () => created;
        // ccfile.WriteText(saved);
        var actual = ccfile.ReadOrWriteText(getValue);
        Assert.Equal(created, actual);
    }

    [Fact]
    public void ReadOrWriteObject_ReturnsExisting()
    {
        ClearFiles();
        var saved = new Cake { Recipe = "storage rocks" };
        var created = new Cake { Recipe = "get new strings!" };
        Func<Cake> getValue = () => created;
        ccfile.WriteValue(saved);
        var actual = ccfile.ReadOrWriteValue(getValue);
        Assert.Equal(saved.Recipe, actual.Recipe);
    }

    [Fact]
    public void ReadOrWriteObject_ReturnsCreated()
    {
        ClearFiles();
        // var saved = new Cake{Recipe = "storage rocks"};
        var created = new Cake { Recipe = "get new strings!" };
        Func<Cake> getValue = () => created;
        // ccfile.WriteValue(saved);
        var actual = ccfile.ReadOrWriteValue(getValue);
        Assert.Equal(created.Recipe, actual.Recipe);
    }

    [Fact]
    public void ReadOrWriteValueType_ReturnsExisting()
    {
        ClearFiles();
        var saved = 9;
        var created = 9123;
        Func<int> getValue = () => created;
        ccfile.WriteValue(saved);
        var actual = ccfile.ReadOrWriteValue<int>(getValue);
        Assert.Equal(saved, actual);
    }

    [Fact]
    public void ReadOrWriteValueType_ReturnsCreated()
    {
        ClearFiles();
        // var saved = 9;
        var created = 9123;
        Func<int> getValue = () => created;
        // ccfile.WriteValue(saved);
        var actual = ccfile.ReadOrWriteValue<int>(getValue);
        Assert.Equal(created, actual);
    }
}

public class Cake
{
    public bool HasRaisins { get; init; } = true;
    public int SliceCount { get; set; } = 6;
    public string Recipe { get; set; } = "Stir loads";
}
