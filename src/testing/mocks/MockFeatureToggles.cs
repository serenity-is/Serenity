namespace Serenity.Testing;

public class MockFeatureToggles : IFeatureToggles
{
    public Func<string, bool>? IsEnabledCallback { get; set; }

    public bool IsEnabled(string feature)
    {
        return IsEnabledCallback?.Invoke(feature) ?? false;
    }
}
