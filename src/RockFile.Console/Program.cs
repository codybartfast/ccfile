using Fmbm.IO;

Action<string> wl = Console.WriteLine;
Console.WriteLine("Hello, World!");

var fi = new FileInfo("relative");
wl(fi.FullName);
wl(Environment.CurrentDirectory);
Environment.CurrentDirectory =
    Directory.GetParent(Environment.CurrentDirectory)!.FullName;
wl(Environment.CurrentDirectory);
wl(fi.FullName);
fi.Refresh();
wl(fi.FullName);
