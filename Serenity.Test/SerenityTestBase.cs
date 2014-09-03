using Newtonsoft.Json;
using Serenity;
using Serenity.Caching;
using System;
using System.IO;
using System.Reflection;

namespace Serenity.Test
{
    public class SerenityTestBase : IDisposable
    {
        private IDisposable iocContext;

        public SerenityTestBase()
        {
            Dependency.SetResolver(new IoC.DependencyResolver());
            iocContext = IoC.StartContext();
            IoC.RegisterInstance<ICache>(new HttpRuntimeCache());
        }

        public virtual void Dispose()
        {
            iocContext.Dispose();
        }
    }
}