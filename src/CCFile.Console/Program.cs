using Fmbm.IO;

Action<string> wl = Console.WriteLine;
Console.WriteLine("Hello, World");

Environment.CurrentDirectory = DirPaths.AppRoot.CheckedPath;
new CCValue<string[]>("CCFile_Sample.txt").Delete();


var ccvalue = new CCValue<List<string>>("CCFile_Sample.txt");

// Serialize a list to disk
ccvalue.Write(new List<string> { "Apple", "Banana", "Cherry" });

// Deserialize file and get last element of list
Console.WriteLine(ccvalue.Read().Last());

// OUTPUT:
// Cherry, Banana, Apple
// Apple, Banana, Cherry
