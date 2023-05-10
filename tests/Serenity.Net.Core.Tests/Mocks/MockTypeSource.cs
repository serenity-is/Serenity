namespace Serenity.Tests;

public class MockTypeSource : ITypeSource
{
    private readonly Type[] types;
    private readonly Attribute[] attributes;

    public MockTypeSource(params Type[] types)
    {
        this.types = types ?? throw new ArgumentNullException(nameof(types));
        attributes = Array.Empty<Attribute>();
    }

    public MockTypeSource(params Attribute[] attributes)
    {
        this.attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
        types = Array.Empty<Type>();
    }

    public IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType)
    {
        return attributes;
    }

    public IEnumerable<Type> GetTypes()
    {
        return types;
    }

    public IEnumerable<Type> GetTypesWithAttribute(Type attributeType)
    {
        return types.Where(x => x.GetCustomAttribute(attributeType) != null);
    }

    public IEnumerable<Type> GetTypesWithInterface(Type interfaceType)
    {
        return types.Where(type => interfaceType.IsAssignableFrom(type));
    }
}