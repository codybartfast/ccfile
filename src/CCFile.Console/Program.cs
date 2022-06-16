using Fmbm.IO;

Action<string> wl = Console.WriteLine;
Console.WriteLine("Hello, World!");

// var ccfile = new CCFile(DirPaths.AppRoot.CheckedPath +"\\AFile.txt");
// var v = ccfile.ReadValue<int>();
// Console.WriteLine(v);

var path = DirPaths.AppRoot.CheckedPath +"\\AFile.txt";
Console.WriteLine(path);