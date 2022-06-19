using Fmbm.IO;

Action<string> wl = Console.WriteLine;
Console.WriteLine("Hello, World!");

Environment.CurrentDirectory = DirPaths.AppRoot.CheckedPath;

var ccvalue = new CCValue<string[]>("CCFile_Sample.txt");

// XXX
ccvalue.Delete();

ccvalue.ReadOrWrite(() => new[] { "Cherry", "Banana", "Apple" });

Console.WriteLine(String.Join(", ", ccvalue.Read()!));

File.ReadAllText("None Suche");

// ccvalue.Modify(fruit =>
// {
//     Array.Sort(fruit!);
//     return fruit;
// });

// OUTPUT:
// Apple
// Apple
