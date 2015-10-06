using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   SQL sorgusundaki bir JOIN ifadesine karşılık gelir (INNER, OUTER, CROSS vs.)</summary>
    public abstract class Join : Alias
    {
        private IDictionary<string, Join> joins;
        private ICriteria onCriteria;
        private HashSet<string> referencedAliases;

        public abstract string GetKeyword();

        protected Join(IDictionary<string, Join> joins, string toTable, string alias, ICriteria onCriteria)
            : base(toTable, alias)
        {
            this.joins = joins;
            this.onCriteria = onCriteria;

            if (!ReferenceEquals(this.onCriteria, null))
            {
                var aliases = JoinAliasLocator.Locate(this.onCriteria.ToStringIgnoreParams());
                if (aliases != null && aliases.Count > 0)
                    referencedAliases = aliases;
            }

            var toTableAliases = JoinAliasLocator.Locate(this.Table);
            if (toTableAliases != null && toTableAliases.Count > 0)
            {
                if (referencedAliases == null)
                    referencedAliases = toTableAliases;
                else
                {
                    foreach (var x in toTableAliases)
                        referencedAliases.Add(x);
                }
            }

            if (joins != null)
            {
                if (joins.ContainsKey(this.Name))
                    throw new ArgumentException(String.Format(
                        "There is already a join with alias '{0}'", this.Name));

                joins.Add(this.Name, this);
            }
        }

        /// <summary>
        ///   Left outer join'in "ON(...)" kısmında yazılan ifadeyi verir.</summary>
        public ICriteria OnCriteria
        {
            get
            {
                return onCriteria;
            }
        }

        /// <summary>
        ///   Left outer join'in "ON(...)" kısmında yazılan ifadedeki alias ların listesini verir.</summary>
        public HashSet<string> ReferencedAliases
        {
            get
            {
                return referencedAliases;
            }
        }

        public IDictionary<string, Join> Joins
        {
            get { return joins; }
        }
    }
}