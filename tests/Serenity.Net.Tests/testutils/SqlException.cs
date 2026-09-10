namespace System.Data;

/// <summary>
/// Test double mimicking a SqlException with a Number property,
/// used to trigger the connection pool exception retry logic in SqlHelper.
/// </summary>
public class SqlException(int number) : Exception($"SqlException #{number}")
{
    public int Number { get; } = number;
}
