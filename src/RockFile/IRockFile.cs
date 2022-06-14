namespace Fmbm.IO;

public interface IRockValue<TValue>
{
    TValue ReadOrWrite(Func<TValue> getValue);
    void Modify(Func<TValue?, TValue> modify);
    TValue? Read();
    void Write(TValue obj);
}

public interface IRockFile
{
    TValue ReadOrWriteValue<TValue>(Func<TValue> getValue);
    TValue ModifyValue<TValue>(Func<TValue?, TValue> modify);
    TValue? ReadValue<TValue>();
    void WriteValue<TValue>(TValue obj);

    string ReadOrWriteText(Func<string> getValue);
    string ModifyText(Func<string?, string> modify);
    string? ReadText();
    void WriteText(string text);

    byte[] ReadOrWriteBytes(Func<Byte[]> getValue);
    byte[] ModifyBytes(Func<byte[]?, byte[]> modify);
    byte[]? ReadBytes();
    void WriteBytes(byte[] bytes);
}

public interface IRockGeneric
{
    TValue ReadOrWriteValue<TValue>(Func<TValue> getValue);
    TValue ModifyValue<TValue>(Func<TValue?, TValue> modify);
    TValue? ReadValue<TValue>();
    void WriteValue<TValue>(TValue obj);
}

public interface IRockText
{
    string ReadOrWriteText(Func<string> getValue);
    string ModifyText(Func<string?, string> modify);
    string? ReadText();
    void WriteText(string text);
}

public interface IRockBinary
{
    byte[] ReadOrWriteBytes(Func<Byte[]> getValue);
    byte[] ModifyBytes(Func<byte[]?, byte[]> modify);
    byte[]? ReadBytes();
    void WriteBytes(byte[] bytes);
}
