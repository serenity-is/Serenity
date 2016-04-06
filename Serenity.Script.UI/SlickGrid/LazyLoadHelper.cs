using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public static class LazyLoadHelper
    {
        public static void ExecuteOnceWhenShown(jQueryObject element, Action callback)
        {
        }

        public static void ExecuteEverytimeWhenShown(jQueryObject element, Action callback,
            bool callNowIfVisible)
        {
        }
    }
}