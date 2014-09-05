using Newtonsoft.Json;
using Serenity;
using Serenity.Caching;
using Serenity.Testing;
using Serenity.Abstractions;
using System;
using System.IO;
using System.Reflection;

namespace Serenity.Test
{
    public class SerenityTestBase : IDisposable
    {
        private MunqContext munqContext;

        public SerenityTestBase()
        {
            munqContext = new MunqContext();
            var registrar = Dependency.Resolve<IDependencyRegistrar>();
            registrar.RegisterInstance<ICache>(new HttpRuntimeCache());
        }

        public void Dispose()
        {
            if (munqContext != null)
                munqContext.Dispose();
        }
    }
}