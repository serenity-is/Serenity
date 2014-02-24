using Serenity.Testing;
using Serenity.Test.Testing;
using System;
using Xunit;

namespace Serenity.Test.Data
{
    public class SerenityDbScript : DbScript
    {
        public SerenityDbScript()
        {
            AddResourceScript("Serenity.Test.SqlScript.DbSerenity.sql");
        }
    }
}