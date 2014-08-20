using System;

namespace Serenity.Services
{
    public interface IConfigurationRepository
    {
        void Save(Type settingType, object value);
        object Load(Type settingType);
    }
}