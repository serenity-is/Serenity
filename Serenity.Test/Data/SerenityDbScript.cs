using Serenity.Testing;
using Serenity.Testing.Test;
using System;
using Xunit;

namespace Serenity.Data.Test
{
    public class SerenityDbScript : DbScript
    {
        public SerenityDbScript()
        {
            AddResourceScript("Serenity.Test.SqlScript.DbSerenity.sql");
        }
    }
}