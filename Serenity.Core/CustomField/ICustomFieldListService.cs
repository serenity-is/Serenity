using Serenity.Services;
using System.Collections.Generic;

namespace Serenity.Data
{
    public interface ICustomFieldListService
    {
        IEnumerable<ICustomFieldDefinition> List(string schema);
    }
}