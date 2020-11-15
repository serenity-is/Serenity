namespace Serenity.Data
{
    /// <summary>
    ///   Basic interface for rows that has an ID field.</summary>
    public interface IIdRow: IEntity
    {
        /// <summary>
        ///   Gets ID field for this row.</summary>
        IIdField IdField { get; }
    }
}