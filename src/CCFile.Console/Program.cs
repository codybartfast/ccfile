using Fmbm.IO;

Action<string> wl = Console.WriteLine;
Console.WriteLine("Hello, World!");

Environment.CurrentDirectory = DirPaths.AppRoot.CheckedPath;

ICCFile ccfile = new CCFile("FMBM_CCFile.txt");

ccfile.WriteText("The cat sat on the mat.");

Console.WriteLine(ccfile.ReadText());
Console.WriteLine(ccfile.ReadBytes()!.Length);

