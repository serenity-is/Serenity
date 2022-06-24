using Serenity.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serenity.Tests.CodeGenerator
{
    public partial class ServerTypingsGeneratorTests
    {
        private ServerTypingsGenerator CreateGenerator()
        {
            var generator = new ServerTypingsGenerator(
                typeof(ServerTypingsGeneratorTests).Assembly.Location);
            generator.RootNamespaces.Add("ServerTypingsTest");
            return generator;
        }

    }
}
