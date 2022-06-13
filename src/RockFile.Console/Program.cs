using Fmbm.IO;

Console.WriteLine("Hello, World!");

IRockFile rock =
    new RockFile(Path.Combine(DirPaths.AppRoot.CheckedPath, "AFile.txt"));

var obj = new Derry();

rock.WriteValue<Derry>(obj);
rock.ModifyValue<Derry>(obj =>
{
    obj!.Erin = "Brokovich";
    return obj;
});
Console.WriteLine(rock.ReadText());
Console.WriteLine(rock.ReadValue<Derry>()!.Erin);
Console.WriteLine(rock.ReadValue<Derry>()!.Orla);

class Derry
{
    public string Erin { get; set; } = "Quinn";
    public string Orla { get; } = "McCool";

}