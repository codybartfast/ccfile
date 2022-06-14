using Fmbm.IO;

Action<string> wl = Console.WriteLine;
Console.WriteLine("Hello, World!");

var rock = new RockFile(DirPaths.AppRoot.CheckedPath +"\\AFile.txt");
var v = rock.ReadValue<int>();
Console.WriteLine(v);