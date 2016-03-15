using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class EditorAttribute : Attribute
    {
        public EditorAttribute()
        {
        }

        [IntrinsicProperty]
        public string Key { get; set; }
    }

    [Imported]
    public class OptionsTypeAttribute : Attribute
    {
        public OptionsTypeAttribute(Type optionsType)
        {
            OptionsType = optionsType;
        }

        [IntrinsicProperty]
        public Type OptionsType { get; private set; }
    }
}