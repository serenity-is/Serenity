using System;
using Serenity.Data;

namespace Serenity.Data
{
    public interface IByIdCache<TRow> where TRow : Row
    {
        TRow ById(long id);
    }
}