namespace Serene.AppServices;

internal class DynamicDataGenerator : BaseDynamicDataGenerator
{
    protected override ITypeSource GetTypeSource()
    {
        return new TypeSource();
    }
}