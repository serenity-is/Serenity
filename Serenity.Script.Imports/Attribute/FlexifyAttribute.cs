namespace Serenity
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Makes dialog use Serenity flexify plugin. Flexify automatically
    /// resizes elements when dialog resize. The amount they grow or shrink
    /// are controlled by x/y flex factors.
    /// </summary>
    [Imported]
    public class FlexifyAttribute : Attribute
    {
    }
}