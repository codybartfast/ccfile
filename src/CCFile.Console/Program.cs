using Fmbm.IO;

Action<string> wl = Console.WriteLine;
Console.WriteLine("Hello, World!");

Environment.CurrentDirectory = DirPaths.AppRoot.CheckedPath;

var ccfile = new CCFile("CCFile_Sample.txt");

// Make sure the file doesn't exist
ccfile.Delete() ;

var result1 = ccfile.ReadOrWriteText(() => "Apple");
Console.WriteLine(result1);

var result2 = ccfile.ReadOrWriteText(() => "Banana");
Console.WriteLine(result2);

// OUTPUT:
// Apple
// Apple
