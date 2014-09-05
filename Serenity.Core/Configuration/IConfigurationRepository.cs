using System;

namespace Serenity.Abstractions
{
    public interface IConfigurationRepository
    {
        void Save(Type settingType, object value);
        object Load(Type settingType);
    }
}