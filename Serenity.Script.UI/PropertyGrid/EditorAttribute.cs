using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class EditorAttribute : Attribute
    {
        public EditorAttribute(string displayName)
        {
            this.DisplayName = displayName;
            this.OptionsType = null;
        }

        public EditorAttribute(string displayName, Type optionsType)
        {
            this.DisplayName = displayName;
            this.OptionsType = optionsType;
        }

        [IntrinsicProperty]
        public string DisplayName { get; private set; }

        [IntrinsicProperty]
        public Type OptionsType { get; private set; }
    }
}