using System;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Serenity.Data
{
    [JsonConverter(typeof(JsonDataEnumConverter))]
    [TypeConverter(typeof(DataEnumConverter<EntityType>))]
    public class EntityType : DataEnum
    {
        protected override string GetEnumType()
        {
            return "EntityType";
        }

        public EntityType(string valueKey) : base(valueKey) { }
        public EntityType(int valueId) : base(valueId) { }

        public static EntityType FromInt32(Int32? value)
        {
            if (value == null)
                return null;
            else
                return new EntityType(value.Value);
        }
    }
}