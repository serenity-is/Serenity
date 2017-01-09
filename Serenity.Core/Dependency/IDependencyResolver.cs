namespace Serenity
{
    public interface IDependencyResolver
    {
        TService Resolve<TService>() where TService : class;
        TService TryResolve<TService>() where TService : class;

#if !COREFX
        TService Resolve<TService>(string name) where TService : class;
        TService TryResolve<TService>(string name) where TService : class;
#endif
    }
}
