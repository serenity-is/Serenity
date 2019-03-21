using System.Collections.Generic;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Base class which other custom filtering types derive from.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.FilteringTypeAttribute" />
    public abstract class CustomFilteringAttribute : FilteringTypeAttribute
    {
        private Dictionary<string, object> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFilteringAttribute"/> class.
        /// </summary>
        /// <param name="filteringType">Type of the filtering.</param>
        public CustomFilteringAttribute(string filteringType)
            : base(filteringType)
        {
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="filteringParams">The filtering parameters.</param>
        public override void SetParams(IDictionary<string, object> filteringParams)
        {
            if (options != null)
                foreach (var opt in options)
                    filteringParams[opt.Key] = opt.Value;
        }

        /// <summary>
        /// Sets the option.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected void SetOption(string key, object value)
        {
            this.options = this.options ?? new Dictionary<string, object>();
            this.options[key] = value;
        }

        /// <summary>
        /// Gets the option.
        /// </summary>
        /// <typeparam name="TType">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
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
