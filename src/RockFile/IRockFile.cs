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

public interface IRockGeneric
{
    void ModifyObject<TObject>(Func<TObject?, TObject> modify);
    TObject? ReadObject<TObject>();
    void WriteObject<TObject>(TObject obj);
}

public interface IRockText
{
    void ModifyText(Func<string, string> modify);
    string ReadText();
    void WriteText(string text);
}

public interface IRockBinary
{
    void ModifyBytes(Func<byte[], byte[]> modify);
    byte[] ReadBytes();
    void WriteBytes(byte[] bytes);
}
