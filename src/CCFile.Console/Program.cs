using Fmbm.IO;

Action<string> wl = Console.WriteLine;
Console.WriteLine("Hello, World");

Environment.CurrentDirectory = DirPaths.AppRoot.CheckedPath;
new CCValue<string[]>("CCFile_Sample.txt").Delete();

// Create new CCFile
var ccfile = new CCFile("CCFile_Sample.txt");
Console.WriteLine(ccfile.Exists);

ccfile.WriteText("");
Console.WriteLine(ccfile.Exists);

ccfile.Delete();
Console.WriteLine(ccfile.Exists);

// OUTPUT:
// False
// True
// False
