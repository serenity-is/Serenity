namespace Serenity
{
    using System;

    /// <summary>
    /// Makes dialog responsive. Responsive dialogs are full width and height 
    /// when viewed in mobile devices.
    /// This attribute also turns on flex-layout class. Please note that
    /// flex layout is only available in IE10+ (actually IE11+) and recent browsers.
    /// http://caniuse.com/#feat=flexbox (about 95% support)
    /// This feature is currently in experimental state.
    /// </summary>
    public class ResponsiveAttribute : Attribute
    {
    }
}