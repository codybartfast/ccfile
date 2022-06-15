namespace Fmbm.IO;

public abstract class CCFileException : Exception
{
    public CCFileException(string message) : base(message) { }
    public CCFileException(string message, Exception innerException)
    : base(message, innerException) { }
}

public class LockFileAlreadyExistsException : CCFileException
{
    public LockFileAlreadyExistsException(string message)
        : base(message) { }
}

public class TempFileAlreadyExistsException : CCFileException
{
    public TempFileAlreadyExistsException(string message)
        : base(message) { }
}

public class BackupExistsWithoutMainException : CCFileException
{
    public BackupExistsWithoutMainException(string message)
        : base(message) { }
}

public class CCFileGetInitialValueException : CCFileException
{
    public CCFileGetInitialValueException(Exception getInitialValueException)
        : base("Exception thrown by 'getInitialValue' Func passed to CCFile",
            getInitialValueException)
    { }
}

public class CCFileModifyException : CCFileException
{
    public CCFileModifyException(Exception modifyException)
        : base("Exception thrown by 'Modify' Func passed to CCFile"
            , modifyException)
    { }
}

public class CCFileArchiveException : CCFileException
{
    public CCFileArchiveException(Exception archiveException)
    : base("Exception thrown by 'Archive' Action passed to CCFile"
        , archiveException)
    { }
}
