using Fmbm.IO;

Console.WriteLine(
    "File persistence with the performance and flexibility of a rock!");

var obj = new object();

lock (obj)
{
    Console.WriteLine("1");
    lock (obj)
    {
        Console.WriteLine("2");
    }
}