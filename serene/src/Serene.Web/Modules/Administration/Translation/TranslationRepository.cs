using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Serenity.Localization;
using System.IO;
using System.Reflection;

namespace Serene.Administration.Repositories;

public class TranslationRepository(IRequestContext context, IWebHostEnvironment hostEnvironment,
    ILocalTextRegistry localTextRegistry, ITypeSource typeSource) : BaseRepository(context)
{
    protected IWebHostEnvironment HostEnvironment { get; } = hostEnvironment;
    protected ILocalTextRegistry LocalTextRegistry { get; } = localTextRegistry;
    protected ITypeSource TypeSource { get; } = typeSource;

    public static string GetUserTextsFilePath(IWebHostEnvironment hostEnvironment, string languageID)
    {
        return Path.Combine(hostEnvironment.ContentRootPath, "App_Data", "texts", 
            "user.texts." + (languageID.TrimToNull() ?? "invariant") + ".json");
    }

    public ListResponse<TranslationItem> List(TranslationListRequest request)
    {
        var result = new ListResponse<TranslationItem>();

        var availableKeys = GetAllAvailableLocalTextKeys();
        var targetLanguageID = request.TargetLanguageID.TrimToNull();
        
        var customTranslations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var textsFilePath = GetUserTextsFilePath(HostEnvironment, targetLanguageID);           
        if (File.Exists(textsFilePath))
        {
            var json = JSON.Parse<Dictionary<string, JToken>>(File.ReadAllText(textsFilePath));
            JsonLocalTextRegistration.ProcessNestedDictionary(json, "", customTranslations);
            foreach (var key in customTranslations.Keys)
                availableKeys.Add(key);
        }

        var sorted = new string[availableKeys.Count];
        availableKeys.CopyTo(sorted);
        Array.Sort(sorted);

        targetLanguageID ??= "";
        var sourceLanguageID = request.SourceLanguageID.TrimToEmpty();

        result.Entities = new List<TranslationItem>();

        static string effective(string key)
        {
            if (key.StartsWith("Navigation.", StringComparison.Ordinal))
            {
                key = key["Navigation.".Length..];
                return key.Split(['/']).Last();
            }
            else if (key.StartsWith("Forms.", StringComparison.Ordinal) &&
                key.Contains(".Categories.", StringComparison.Ordinal))
            {
                return key.Split(['.']).Last().TrimToNull();
            }

            return key;
        }

        foreach (var key in sorted)
        {
            if (!customTranslations.TryGetValue(key, out string customText))
                customText = null;

            result.Entities.Add(new TranslationItem
            {
                Key = key,
                SourceText = LocalTextRegistry.TryGet(sourceLanguageID, key, false) ?? effective(key),
                TargetText = LocalTextRegistry.TryGet(targetLanguageID, key, false) ?? effective(key),
                CustomText = customText
            });
        }

        return result;
    }

    public HashSet<string> GetAllAvailableLocalTextKeys()
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (NavigationItemAttribute attr in TypeSource.GetAssemblyAttributes<NavigationItemAttribute>())
            result.Add("Navigation." + (attr.Category.IsEmptyOrNull() ? "" : attr.Category + "/") + attr.Title);

        foreach (var type in TypeSource.GetTypesWithAttribute(typeof(FormScriptAttribute)))
        {
            var attr = type.GetAttribute<FormScriptAttribute>();
            foreach (var member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public))
            {
                var category = member.GetCustomAttribute<CategoryAttribute>();
                if (category != null && !category.Category.IsEmptyOrNull())
                    result.Add("Forms." + attr.Key + ".Categories." + category.Category);
            }
        }

        if (LocalTextRegistry is LocalTextRegistry repository)
            result.AddRange(repository.GetAllTextKeys(false));

        return result;
    }

    public SaveResponse Update(TranslationUpdateRequest request, IServiceProvider services)
    {
        if (request.Translations == null)
            throw new ArgumentNullException(nameof(request.Translations));

        var translations = List(new TranslationListRequest
        {
            SourceLanguageID = request.TargetLanguageID,
        }).Entities.ToDictionary(x => x.Key, x => x.CustomText);

        foreach (var item in request.Translations)
            translations[item.Key] = item.Value;

        var result = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in translations)
            if (!pair.Value.IsEmptyOrNull())
                result.Add(pair.Key, pair.Value);

        string json = JSON.StringifyIndented(result);

        var textsFilePath = GetUserTextsFilePath(HostEnvironment, request.TargetLanguageID);
        Directory.CreateDirectory(Path.GetDirectoryName(textsFilePath));
        File.WriteAllText(textsFilePath, json);

        (LocalTextRegistry as IRemoveAll)?.RemoveAll();
        Startup.InitializeLocalTexts(services);

        Cache.ExpireGroupItems(UserRow.Fields.GenerationKey);
        services.GetService<IDynamicScriptManager>()?.Reset();

        return new SaveResponse();
    }
}