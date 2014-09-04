namespace Serenity
{
    using System;

    public static class Dependency
    {
        private static IDependencyResolver resolver;

        public static TType Resolve<TType>() where TType : class
        {
            return Resolver.Resolve<TType>();
        }

        public static TType Resolve<TType>(string name) where TType : class
        {
            return Resolver.Resolve<TType>(name);
        }

        public static IDependencyResolver SetResolver(IDependencyResolver value)
        {
            var old = resolver;
            resolver = value;
            return old;
        }

        public static TType TryResolve<TType>() where TType : class
        {
            return resolver == null ? null : Resolver.TryResolve<TType>();
        }

        public static TType TryResolve<TType>(string name) where TType : class
        {
            return resolver == null ? null : Resolver.TryResolve<TType>(name);
        }

        public static bool HasResolver
        {
            get { return resolver != null; }
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
    }
}