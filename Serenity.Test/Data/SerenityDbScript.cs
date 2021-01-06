using Serenity.Testing;
using Serenity.Test.Testing;
using System;
using Xunit;
using Serenity.Data;

namespace Serenity.Test.Data
{
    [ConnectionKey("Serenity"), DatabaseAlias("DBSerenity")]
    public class SerenityDbScript : DbScript
    {
        public SerenityDbScript()
        {
            AddResourceScript("Serenity.Test.SqlScript.DbSerenity.sql");
        }
    }
}