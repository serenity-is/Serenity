namespace Serenity.Testing;

public static partial class TestServices
{
    public static void Using(Action<IServiceCollection> setup, Action<IServiceProvider> action)
    {
        var serviceCollection = new ServiceCollection();
        setup(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        action(serviceProvider);
    }

    public async static Task UsingAsync(Action<IServiceCollection> setup, Func<IServiceProvider, Task> actionAsync)
    {
        var serviceCollection = new ServiceCollection();
        setup(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        await actionAsync(serviceProvider);
    }

    public static void UsingRowFields(Action<IServiceCollection> setup, Action<IServiceProvider> action)
    {
        Using(collection =>
        {
            collection.AddSingleton<IRowFieldsProvider, DefaultRowFieldsProvider>();
            setup(collection);
        },
        services =>
        {
            var oldFieldsProvider = RowFieldsProvider.SetLocalFrom(services);
            try
            {
                action(services);
            }
            finally
            {
                RowFieldsProvider.SetLocal(oldFieldsProvider);
            }
        });
    }

    public async static Task UsingRowFieldsAsync(Action<IServiceCollection> setup, Func<IServiceProvider, Task> actionAsync)
    {
        await UsingAsync(collection =>
        {
            collection.AddSingleton<IRowFieldsProvider, DefaultRowFieldsProvider>();
            setup(collection);
        }, async services =>
        {
            var oldFieldsProvider = RowFieldsProvider.SetLocalFrom(services);
            try
            {
                await actionAsync(services);
            }
            finally
            {
                RowFieldsProvider.SetLocal(oldFieldsProvider);
            }
        });
    }

    public static void UsingAnnotatedRows(Assembly[] assemblies, Action<IServiceCollection> setup, Action<IServiceProvider> action)
    {
        UsingRowFields(collection =>
        {
            collection.AddTypeSource(assemblies);
            collection.AddAnnotationTypes();
            setup(collection);
        },
        action);
    }

    public async static Task UsingAnnotatedRowsAsync(Assembly[] assemblies, Action<IServiceCollection> setup, Func<IServiceProvider, Task> actionAsync)
    {
        await UsingRowFieldsAsync(collection =>
        {
            collection.AddTypeSource(assemblies);
            collection.AddAnnotationTypes();
            setup(collection);
        }, actionAsync);
    }
}
