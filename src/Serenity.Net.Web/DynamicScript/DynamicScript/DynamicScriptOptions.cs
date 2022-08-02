namespace Serenity.Web
{
    public class DynamicScriptOptions
    {
        public string DynamicScriptMiddlewareAuthenticationScheme { get; set; }
        /// <summary>
        /// Should middleware fallback to default authentication scheme or force the given scheme.
        /// </summary>
        public bool ShouldForceAuthenticationScheme { get; set; }
    }
}
