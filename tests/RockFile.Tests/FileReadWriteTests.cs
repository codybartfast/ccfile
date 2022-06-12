using System.Text;

namespace Fmbm.IO.Tests;

public class FileReadWriteTests
{
    readonly Encoding encoding = Encoding.UTF8;

    readonly RockFile rock;

    public FileReadWriteTests()
    {
        rock = new RockFile(
            Path.Combine(DirPaths.AppRoot.CheckedPath, "FileWrite.txt"));
    }

    void ClearFiles()
    {
        File.Delete(rock.LockPath);
        File.Delete(rock.TempPath);
        File.Delete(rock.FilePath);
        File.Delete(rock.BackupPath);
    }

    [Fact]
    public void WriteBytes_WriteToDisk()
    {
        ClearFiles();
        var text = DateTime.UtcNow.ToString("o") + "WritesToDisk";
        var bytes = encoding.GetBytes(text);
        rock.WriteBytes(bytes);
        var diskBytes = File.ReadAllBytes(rock.FilePath);
        Assert.Equal(bytes, diskBytes);
    }

    [Fact]
    public void ReadBytes_ReadsFromDisk()
    {
        ClearFiles();
        var text = DateTime.UtcNow.ToString("o") + "ReadsFromDisk";
        var bytes = encoding.GetBytes(text);
        File.WriteAllBytes(rock.FilePath, bytes);
        var rockBytes = rock.ReadBytes();
        Assert.Equal(bytes, rockBytes);
    }

    [Fact]
    public void Text_IsWrittenAndReadFromDisk()
    {
        ClearFiles();
        rock.WriteText("Apple Banana Cherry");
        var diskText = File.ReadAllText(rock.FilePath);
        File.WriteAllText(rock.FilePath,
            diskText.Replace("Banana", "Blueberry"));
        Assert.Equal("Apple Blueberry Cherry", rock.ReadText());
    }

    [Fact]
    public void Object_ReferencesNotEquel()
    {
        ClearFiles();
        var cake = new Cake();
        rock.WriteObject<Cake>(cake);
        var cake2 = rock.ReadObject<Cake>()!;
        Assert.NotSame(cake, cake2);
        Assert.Equal(cake.HasRaisins, cake2.HasRaisins);
        Assert.Equal(cake.SliceCount, cake2.SliceCount);
        Assert.Equal(cake.Recipe, cake2.Recipe);
    }

    [Fact]
    public void Object_IsWrittenAndReadFromDisk()
    {
        ClearFiles();
        var cake = new Cake();
        rock.WriteObject<Cake>(cake);
        var diskText = File.ReadAllText(rock.FilePath);
        File.WriteAllText(rock.FilePath, diskText.Replace("Stir", "Mix"));
        var cake2 = rock.ReadObject<Cake>()!;
        Assert.Equal(true, cake2.HasRaisins);
        Assert.Equal(6, cake2.SliceCount);
        Assert.NotEqual(cake.Recipe, cake2.Recipe);
        Assert.Equal("Mix loads", cake2.Recipe);
    }

    [Fact]
    public void File_IsBackedUp()
    {
        ClearFiles();
        rock.WriteText("Apple");
        Assert.Equal("Apple", File.ReadAllText(rock.FilePath));
        Assert.False(File.Exists(rock.BackupPath));

        rock.WriteText("Banana");
        Assert.Equal("Banana", File.ReadAllText(rock.FilePath));
        Assert.Equal("Apple", File.ReadAllText(rock.BackupPath));

        rock.WriteText("Cherry");
        Assert.Equal("Cherry", File.ReadAllText(rock.FilePath));
        Assert.Equal("Banana", File.ReadAllText(rock.BackupPath));
    }

    [Fact]
    public void Object_Modify_IsCalled()
    {
        var newRecipe = "Bake well";
        var cake = new Cake();
        Assert.NotEqual(newRecipe, cake.Recipe);
        rock.WriteObject<Cake>(cake);
        rock.ModifyObject<Cake>(cake =>
        {
            cake!.SliceCount = 177;
            cake.Recipe = newRecipe;
            return cake;
        });
        var readCake = rock.ReadObject<Cake>()!;
        Assert.Equal(newRecipe, readCake.Recipe);
        Assert.Equal(177, readCake.SliceCount);
    }

    [Fact]
    public void Text_Modify_IsCalled()
    {
        ClearFiles();
        rock.WriteText("One Two Three");
        rock.ModifyText(text => text.Replace("Two", "And"));
        Assert.Equal("One And Three", rock.ReadText());
    }

    [Fact]
    public void Binary_Modify_IsCalled()
    {
        ClearFiles();
        rock.WriteBytes(encoding.GetBytes("Cat"));
        rock.ModifyBytes(bytes =>
        {
            bytes[0] = 66;
            return bytes;
        });
        Assert.Equal("Bat", encoding.GetString(rock.ReadBytes()));
    }

    [Fact]
    public void AfterWrite_ArchiveIsCalled()
    {
        string? fileContent = null;
        string? backupContent = null;

        ClearFiles();
        var arcRock = new RockFile(this.rock.FilePath, Archive);

        arcRock.WriteText("Apple Pie");
        Assert.Equal(fileContent, "Apple Pie");
        Assert.Null(backupContent);

        arcRock.WriteText("Banana Split");
        Assert.Equal(fileContent, "Banana Split");
        Assert.Equal(backupContent, "Apple Pie");

        arcRock.WriteText("Cherry Cola");
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
}

class Cake
{
    public bool HasRaisins { get; init; } = true;
    public int SliceCount { get; set; } = 6;
    public string Recipe { get; set; } = "Stir loads";
}
