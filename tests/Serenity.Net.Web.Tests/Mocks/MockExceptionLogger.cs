namespace Serenity.Tests;

public class MockExceptionLogger : IExceptionLogger
{
    public Exception LastException { get; private set; }
    public string LastCategory { get; private set; }
    
    public void Log(Exception exception, string category)
    {
        LastException = exception;
        LastCategory = category;
    }
}