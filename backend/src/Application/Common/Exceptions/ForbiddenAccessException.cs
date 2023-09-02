namespace Application.Common.Exceptions;

public sealed class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message):base(message)
    {
        
    }
    
    public ForbiddenAccessException(string message,Exception exception):base(message,exception)
    {
        
    }
}