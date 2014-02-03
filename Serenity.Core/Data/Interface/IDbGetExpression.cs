namespace Serenity.Data
{
    /// <summary>
    ///   Interface for query classes (e.g. SqlQuery) that can return expression given fieldname
    /// </summary>
    public interface IDbGetExpression
    {
        string GetExpression(string fieldName);
    }
}
