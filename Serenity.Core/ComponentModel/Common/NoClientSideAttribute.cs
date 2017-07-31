using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Add this attribute on Enum declaration or on Enum value
    /// to avoid the ServerTypingsGenertor to create client side value
    /// </summary>
    public class NoClientSideAttribute : Attribute
    {
        public NoClientSideAttribute()
        {
        }
    }
}