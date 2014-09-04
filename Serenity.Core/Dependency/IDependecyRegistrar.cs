namespace Serenity
{
    public interface IDependencyRegistrar
    {
        object RegisterInstance<TType>(TType instance) where TType : class;
        object RegisterInstance<TType>(string name, TType instance) where TType : class;
        object Register<TType, TImpl>() where TType : class where TImpl : class, TType;
        object Register<TType, TImpl>(string name) where TType : class where TImpl : class, TType;
    }
}