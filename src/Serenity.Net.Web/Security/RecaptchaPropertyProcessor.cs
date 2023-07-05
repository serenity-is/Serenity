using Microsoft.Extensions.Options;
using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Property processor to pass recaptcha site key to client side
/// </summary>
public class RecaptchaPropertyProcessor : PropertyProcessor
{
    private readonly IOptions<RecaptchaSettings> options;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    public RecaptchaPropertyProcessor(IOptions<RecaptchaSettings> options = null)
    {
        this.options = options;
    }

    /// <inheritdoc/>
    public override void Process(IPropertySource source, PropertyItem item)
    {
        if (item.EditorType == RecaptchaAttribute.Key &&
            (item.EditorParams == null ||
             !item.EditorParams.ContainsKey("siteKey")))
        {
            var siteKey = options?.Value?.SiteKey;
            if (string.IsNullOrEmpty(siteKey) &&
                string.IsNullOrEmpty(options?.Value?.SecretKey))
            {
                item.HideOnInsert = true;
                item.HideOnUpdate = true;
                item.Visible = false;
                item.Required = false;
                item.EditorType = "String";
            }
            else
            {
                item.EditorParams ??= new();
                item.EditorParams["siteKey"] = siteKey;
            }
        }
    }

    /// <inheritdoc/>
    public override int Priority => 11;
}