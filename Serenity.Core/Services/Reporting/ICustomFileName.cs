using System.Collections;
using System.Collections.Generic;

namespace Serenity.Reporting
{
    public interface ICustomFileName : IReport
    {
        string GetFileName();
    }
}