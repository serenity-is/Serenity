using Microsoft.Extensions.Options;
using Serenity.PropertyGrid;

namespace Serenity.Web;

/// <summary>
/// Property processor to pass recaptcha site key to client side
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
public class RecaptchaPropertyProcessor(IOptions<RecaptchaSettings> options = null) : PropertyProcessor
{
    private readonly IOptions<RecaptchaSettings> options = options;

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
                item.EditorParams ??= [];
                item.EditorParams["siteKey"] = siteKey;
            }
        }
    }

    /// <inheritdoc/>
    public override int Priority => 11;
}