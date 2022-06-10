namespace Fmbm.IO;

public abstract class RockFileException : Exception
{

}

public class LockFileAlreadyExistsException : RockFileException
{

}

public class TemporaryFileExistsException : RockFileException
{

}

public class BackupExistsWithoutOriginalException : RockFileException
{

}

