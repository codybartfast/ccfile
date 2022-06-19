using Fmbm.IO;

Action<string> wl = Console.WriteLine;
Console.WriteLine("Hello, World");

Environment.CurrentDirectory = DirPaths.AppRoot.CheckedPath;
new CCValue<string[]>("CCFile_Sample.txt").Delete();

//////////////////////////

void Archive(string filePath, string? backPath)
{
    var after = File.ReadAllText(filePath);
    var before = backPath is null ? "<none>" : File.ReadAllText(backPath);

    Console.Write($@"
** File Updated **
==================
Before: {before}
After: {after}
");
}

var ccvalue =
    new CCValue<Dictionary<string, string>>("CCFile_Sample.txt", Archive);

ccvalue.ReadOrWrite(() =>
    new Dictionary<string, string> { { "A", "Apple" }, { "B", "Banana" } });

ccvalue.Modify(fruit =>
{
    fruit.Add("C", "Cherry");
    return fruit;
});

// OUTPUT:
// False
// True
// False
