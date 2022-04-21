namespace Serenity.Services
{
    public class DefaultHandlerRegistry : IDefaultHandlerRegistry
    {
        private readonly ITypeSource typeSource;

        public DefaultHandlerRegistry(ITypeSource typeSource)
        {
            this.typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));
        }

        public virtual IEnumerable<Type> GetTypes()
        {
            return typeSource.GetTypesWithInterface(typeof(IRequestHandler))
                .Where(type => !type.IsInterface && !type.IsAbstract);
        }

        public IEnumerable<Type> GetTypes(Type handlerType)
        {
            return GetTypes().Where(type => handlerType.IsAssignableFrom(type));
        }
    }
}