namespace Serenity.Navigation;

/// <summary>
/// Factory abstraction for navigation models
/// </summary>
public interface INavigationModelFactory
{
    /// <summary>
    /// Creates a navigation model for current user
    /// </summary>
    INavigationModel Create();
}