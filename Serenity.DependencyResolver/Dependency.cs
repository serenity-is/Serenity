using System;

namespace Serenity
{
    public static class Dependency
    {
        private static IDependencyResolver resolver;

        public static IDependencyResolver SetResolver(IDependencyResolver value)
        {
            resolver = value;
            return resolver;
        }

        public static IDependencyResolver Resolver
        {
            get
            {
                var current = resolver;

                if (current == null)
                    throw new InvalidProgramException(
                        "Default IDependencyResolver implementation is not set. Use Serenity.Dependency.SetResolver " +
                        "to set your dependency resolver implementation.");

                return current;
            }
        }

        public static TType Resolve<TType>() where TType : class
        {
            return Resolver.Resolve<TType>();
        }

        public static TType Resolve<TType>(string name) where TType : class
        {
            return Resolver.Resolve<TType>(name);
        }

        public static TType TryResolve<TType>() where TType : class
        {
            return Resolver.TryResolve<TType>();
        }

        public static TType TryResolve<TType>(string name) where TType : class
        {
            return Resolver.TryResolve<TType>(name);
        }
    }
}