using System;

namespace Serenity.Abstractions
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}