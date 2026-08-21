namespace Serenity.Navigation;

/// <summary>
/// Factory abstraction for navigation models.
/// </summary>
public interface INavigationModelFactory
{
    /// <summary>
    /// Creates a navigation model for the current user.
    /// </summary>
    INavigationModel Create();
}