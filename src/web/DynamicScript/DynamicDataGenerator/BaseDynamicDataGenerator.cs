using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Extensions.DependencyInjection;
using Serenity.PropertyGrid;
using Serenity.Reflection;
using System.IO;

namespace Serenity.Web;

/// <summary>
/// Base class for generating .json files under dynamic-data folder for script testing purposes
/// </summary>
public abstract class BaseDynamicDataGenerator
{
    /// <summary>
    /// Generates .json files under dynamic-data folder for script testing purposes
    /// </summary>
    public virtual void Run()
    {
        var collection = new ServiceCollection();
        collection.TryAddSingleton(GetTypeSource());
        AddServices(collection);

        var services = collection.BuildServiceProvider();

        RowFieldsProvider.SetDefaultFrom(services);

        InitializeScripts(services);

        var dynamicDataFolder = GetDynamicDataFolder();
        Directory.CreateDirectory(dynamicDataFolder);

        var scriptManager = services.GetRequiredService<IDynamicScriptManager>();

        foreach (var name in scriptManager.GetRegisteredScriptNames())
        {
            if (ShouldSkipScript(name))
                continue;

            var content = scriptManager.GetScriptText(name, json: true);

            if (name.StartsWith("Columns.") || name.StartsWith("Form."))
            {
                content = Newtonsoft.Json.JsonConvert.SerializeObject(
                    Newtonsoft.Json.JsonConvert.DeserializeObject<PropertyItemsData>(content),
                    Newtonsoft.Json.Formatting.Indented, JsonSettings.Tolerant);
            }
            else
            {
                content = Newtonsoft.Json.JsonConvert.SerializeObject(
                    Newtonsoft.Json.JsonConvert.DeserializeObject(content),
                    Newtonsoft.Json.Formatting.Indented);
            }
            var target = Path.Combine(dynamicDataFolder, name) + ".json";

            if (File.Exists(target))
            {
                var existing = File.ReadAllText(target);
                if (existing == content)
                    continue;
            }

            File.WriteAllText(target, content);
        }
    }

    /// <summary>
    /// Returns true if the script should be skipped
    /// </summary>
    /// <param name="name">Script name</param>
    protected virtual bool ShouldSkipScript(string name)
    {
        return name == "ColumnsBundle" ||
            name == "FormBundle" ||
            name == "ColumnAndFormBundle" ||
            name == "RegisteredScripts";
    }

    /// <summary>
    /// Initializes scripts
    /// </summary>
    /// <param name="services">Service provider</param>
    protected virtual void InitializeScripts(IServiceProvider services)
    {
        var scriptManager = services.GetRequiredService<IDynamicScriptManager>();
        var propertyProvider = services.GetRequiredService<IPropertyItemProvider>();
        var typeSource = services.GetRequiredService<ITypeSource>();
        ColumnsScriptRegistration.RegisterColumnsScripts(scriptManager, typeSource, propertyProvider, services);
        FormScriptRegistration.RegisterFormScripts(scriptManager, typeSource, propertyProvider, services);
    }

    /// <summary>
    /// Initializes services
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    protected virtual IServiceCollection AddServices(IServiceCollection collection)
    {
        collection.AddMemoryCache();
        collection.AddDistributedMemoryCache();
        collection.AddSingleton<IAnnotationTypeRegistry, AnnotationTypeRegistry>();
        collection.AddSingleton<IDynamicScriptManager, DynamicScriptManager>();
        collection.AddSingleton<IPermissionService, NullPermissionService>();
        collection.AddSingleton<IPropertyItemProvider, DefaultPropertyItemProvider>();
        collection.AddSingleton<IRowTypeRegistry, DefaultRowTypeRegistry>();
        collection.AddSingleton<IRowFieldsProvider, DefaultRowFieldsProvider>();
        collection.AddSingleton<ITextLocalizer>(NullTextLocalizer.Instance);
        collection.AddSingleton<ITwoLevelCache, TwoLevelCache>();
        return collection;
    }

    /// <summary>
    /// Gets the project rool folder
    /// </summary>
    protected virtual string GetProjectRoot()
    {
        return Environment.CurrentDirectory;
    }

    /// <summary>
    /// Gets the dynamic-data folder
    /// </summary>
    protected virtual string GetDynamicDataFolder()
    {
        var projectRoot = GetProjectRoot();
        return Path.Combine(projectRoot, "dynamic-data");
    }

    /// <summary>
    /// Gets the type source
    /// </summary>
    protected virtual ITypeSource GetTypeSource()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            EnvironmentName = "Development",
            ApplicationName = GetType().Assembly.GetName().Name
        });
        return builder.Services.AddApplicationPartsTypeSource();
    }

    /// <summary>
    /// Checks the arguments, if it is "dynamic-data" runs the generator and exits
    /// </summary>
    /// <param name="args">Arguments</param>
    public virtual void RunAndExitIf(string[] args)
    {
        if (args?.Length == 1 && args[0] == "dynamic-data")
        {
            Run();
            Environment.Exit(0);
        }
    }
}

internal class NullPermissionService : IPermissionService
{
    public bool HasPermission(string permission)
    {
        return true;
    }
}