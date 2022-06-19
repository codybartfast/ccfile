namespace Fmbm.IO;

public interface ICCValue<TValue>
{
    TValue ReadOrWrite(Func<TValue> getValue);
    void Modify(Func<TValue, TValue> modify);
    TValue Read();
    void Write(TValue obj);
    bool Exists { get; }
}

public interface ICCFile : ICCBinary, ICCText, ICCGeneric { }

public interface ICCGeneric
{
    TValue ReadOrWriteValue<TValue>(Func<TValue> getValue);
    TValue ModifyValue<TValue>(Func<TValue, TValue> modify);
    TValue ReadValue<TValue>();
    void WriteValue<TValue>(TValue obj);
    bool Exists { get; }
}

public interface ICCText
{
    string ReadOrWriteText(Func<string> getValue);
    string ModifyText(Func<string, string> modify);
    string ReadText();
    void WriteText(string text);
    bool Exists { get; }
}

public interface ICCBinary
{
    byte[] ReadOrWriteBytes(Func<Byte[]> getValue);
    byte[] ModifyBytes(Func<byte[], byte[]> modify);
    byte[] ReadBytes();
    void WriteBytes(byte[] bytes);
    bool Exists { get; }
}
