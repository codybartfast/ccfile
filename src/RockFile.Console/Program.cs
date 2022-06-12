using Fmbm.IO;

Console.WriteLine("Hello, World!");

var rock =
    new RockFile(Path.Combine(DirPaths.AppRoot.CheckedPath, "AFile.txt"));

var obj = new Derry();

rock.WriteObject<Derry>(obj);
rock.ModifyObject<Derry>(obj =>
{
    obj!.Erin = "Brokovich";
    return obj;
});
Console.WriteLine(rock.ReadText());
Console.WriteLine(rock.ReadObject<Derry>()!.Erin);
Console.WriteLine(rock.ReadObject<Derry>()!.Orla);

class Derry
{
    public string Erin { get; set; } = "Quinn";
    public string Orla { get; } = "McCool";
}