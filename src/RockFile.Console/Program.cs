using Fmbm.IO;
using Fmbm.Paths;

Console.WriteLine("Hello, World!");


RockFile rock = new RockFile(
    Path.Combine(DirPaths.AppRoot.CheckedPath, "CatNames.lst"));

Console.WriteLine(rock.FilePath);
