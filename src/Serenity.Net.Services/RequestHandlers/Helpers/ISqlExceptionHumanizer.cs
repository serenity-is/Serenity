using System;

namespace Serenity.Data
{
    public interface ISqlExceptionHumanizer
    {
        void Humanize(Exception exception, IRow row);
    }
}