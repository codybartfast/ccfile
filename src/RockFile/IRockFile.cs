namespace Fmbm.IO;

public interface IRockFile
{
    void ModifyObject<TObject>(Func<TObject?, TObject> modify);
    TObject? ReadObject<TObject>();
    void WriteObject<TObject>(TObject obj);

    void ModifyText(Func<string, string> modify);
    string ReadText();
    void WriteText(string text);

    void ModifyBytes(Func<byte[], byte[]> modify);
    byte[] ReadBytes();
    void WriteBytes(byte[] bytes);
}

public interface IRockObjectFile
{
    void ModifyObject<TObject>(Func<TObject?, TObject> modify);
    TObject? ReadObject<TObject>();
    void WriteObject<TObject>(TObject obj);
}

public interface IRockTextFile
{
    void ModifyText(Func<string, string> modify);
    string ReadText();
    void WriteText(string text);
}

public interface IRockBinaryFile
{
    void ModifyBytes(Func<byte[], byte[]> modify);
    byte[] ReadBytes();
    void WriteBytes(byte[] bytes);
}
