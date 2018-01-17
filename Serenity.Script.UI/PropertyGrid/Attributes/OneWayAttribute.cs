using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class OneWayAttribute : Attribute
    {
        public OneWayAttribute()
        {
        }
    }
}