namespace Serenity.Data
{
    /// <summary>
    ///   Interface for query classes (e.g. SqlQuery) that can return expression given column name
    /// </summary>
    public interface IGetExpressionByName
    {
        string GetExpression(string columnName);
    }
}
