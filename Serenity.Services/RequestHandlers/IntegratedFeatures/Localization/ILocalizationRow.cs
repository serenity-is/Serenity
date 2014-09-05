using System;

namespace Serenity.Data
{
    public interface ILocalizationRow : IIdRow
    {
        Int32Field CultureIdField { get; }
    }
}