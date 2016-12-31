using System.Collections.Generic;

namespace Serenity.ComponentModel
{
    public abstract class CustomFormatterAttribute : FormatterTypeAttribute
    {
        private Dictionary<string, object> options;

        public CustomFormatterAttribute(string formatterType)
            : base(formatterType)
        {
        }

        public override void SetParams(IDictionary<string, object> formatterParams)
        {
            if (options != null)
                foreach (var opt in options)
                    formatterParams[opt.Key] = opt.Value;
        }

        protected void SetOption(string key, object value)
        {
            this.options = this.options ?? new Dictionary<string, object>();
            this.options[key] = value;
        }

        protected TType GetOption<TType>(string key)
        {
            if (this.options == null)
                return default(TType);

            object obj;
            if (!this.options.TryGetValue(key, out obj) || obj == null)
                return default(TType);

            return (TType)obj;
        }
    }
}
