namespace Serenity.Extensions.DependencyInjection;

public class PasswordStrengthServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPasswordStrengthValidator_Registers_Singleton()
    {
        var services = new ServiceCollection();
        var result = services.AddPasswordStrengthValidator();

        Assert.Same(services, result);
        Assert.Contains(services, x =>
            x.ServiceType == typeof(IPasswordStrengthValidator) &&
            x.ImplementationType == typeof(PasswordStrengthValidator) &&
            x.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddPasswordStrengthValidator_Does_Not_Replace_Existing()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IPasswordStrengthValidator>(new NullPasswordStrengthValidator());

        services.AddPasswordStrengthValidator();

        Assert.DoesNotContain(services, x =>
            x.ServiceType == typeof(IPasswordStrengthValidator) &&
            x.ImplementationType == typeof(PasswordStrengthValidator));
    }
}
