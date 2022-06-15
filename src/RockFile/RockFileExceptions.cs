namespace Fmbm.IO;

public abstract class RockFileException : Exception
{
    public RockFileException(string message) : base(message) { }
    public RockFileException(string message, Exception innerException)
    : base(message, innerException) { }
}

public class LockFileAlreadyExistsException : RockFileException
{
    public LockFileAlreadyExistsException(string message)
        : base(message) { }
}

public class TempFileAlreadyExistsException : RockFileException
{
    public TempFileAlreadyExistsException(string message)
        : base(message) { }
}

public class BackupExistsWithoutMainException : RockFileException
{
    public BackupExistsWithoutMainException(string message)
        : base(message) { }
}

public class RockFileGetInitialValueException : RockFileException
{
    public RockFileGetInitialValueException(Exception getInitialValueException)
        : base("Exception thrown by 'getInitialValue' Func passed to RockFile",
            getInitialValueException)
    { }
}

public class RockFileModifyException : RockFileException
{
    public RockFileModifyException(Exception modifyException)
        : base("Exception thrown by 'Modify' Func passed to RockFile"
            , modifyException)
    { }
}

public class RockFileArchiveException : RockFileException
{
    public RockFileArchiveException(Exception archiveException)
    : base("Exception thrown by 'Archive' Action passed to RockFile"
        , archiveException)
    { }
}
