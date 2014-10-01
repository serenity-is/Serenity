using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Serenity.Data
{
    /// <summary>
    ///   Serialize/deserialize a BaseCriteria object and checks for safety of criteria expressions.</summary>
    public class JsonSafeCriteriaConverter : JsonCriteriaConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            var value = (BaseCriteria)base.ReadJson(reader, objectType, existingValue, serializer);
            
            if (Object.ReferenceEquals(null, value))
                return value;

            new SafeCriteriaValidator().Validate(value);

            return value;
        }
    }
}