namespace RedisStorm.Exceptions;

public class ConnectionMultiplexerException : Exception
{
    private const string ErrorMessage = "Connection multiplexer is not set!";

    public ConnectionMultiplexerException() : base(ErrorMessage)
    {
    }
}