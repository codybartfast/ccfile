namespace Fmbm.IO;

public interface IRockFile
{
    void ModifyValue<TValue>(Func<TValue?, TValue> modify);
    TValue? ReadValue<TValue>();
    void WriteValue<TValue>(TValue obj);

    void ModifyText(Func<string?, string> modify);
    string? ReadText();
    void WriteText(string text);

    void ModifyBytes(Func<byte[]?, byte[]> modify);
    byte[]? ReadBytes();
    void WriteBytes(byte[] bytes);
}

public interface IRockFileValue<TValue>
{
    void Modify(Func<TValue?, TValue> modify);
    TValue? Read();
    void Write(TValue obj);
}

public interface IRockGeneric
{
    void ModifyValue<TValue>(Func<TValue?, TValue> modify);
    TValue? ReadValue<TValue>();
    void WriteValue<TValue>(TValue obj);
}

public interface IRockText
{
    void ModifyText(Func<string?, string> modify);
    string? ReadText();
    void WriteText(string text);
}

public interface IRockBinary
{
    void ModifyBytes(Func<byte[]?, byte[]> modify);
    byte[]? ReadBytes();
    void WriteBytes(byte[] bytes);
}
