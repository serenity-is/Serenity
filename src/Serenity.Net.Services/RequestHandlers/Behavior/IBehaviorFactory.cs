namespace Serenity.Services
{
    public interface IBehaviorFactory
    {
        object CreateInstance(Type behaviorType);
    }
}