namespace Fmbm.IO;

public abstract class RockFileException : Exception
{
    public RockFileException(string message) : base(message) { }
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
