using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static class WidgetPrototype
    {
        [IncludeGenericArguments(false), InlineCode("{widget}.init({action})")]
        public static TWidget Init<TWidget>(this TWidget widget, Action<TWidget> action = null)
            where TWidget : Widget
        {
            return null;
        }
    }
}
