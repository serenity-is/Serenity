using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   SQL sorgusundaki bir JOIN ifadesine karþýlýk gelir (INNER, OUTER, CROSS vs.)</summary>
    public abstract class Join : Alias
    {
        private Dictionary<string, Join> joins;
        private BaseCriteria onCriteria;
        private string onCriteriaString;
        private HashSet<string> referencedAliases;

        public abstract string GetKeyword();

        protected Join(IDictionary<string, Join> joins, string toTable, string alias, BaseCriteria onCriteria)
            : base(alias, toTable)
        {
            this.joins = new Dictionary<string, Join>();
            this.onCriteria = onCriteria;

            if (!Object.ReferenceEquals(this.onCriteria, null))
            {
                this.onCriteriaString = this.onCriteria.ToString();

                var aliases = JoinAliasLocator.Locate(this.onCriteriaString);
                if (aliases != null && aliases.Count > 0)
                    referencedAliases = aliases;
            }

            var toTableAliases = JoinAliasLocator.Locate(this.Table);
            if (toTableAliases != null && toTableAliases.Count > 0)
            {
                if (referencedAliases == null)
                    referencedAliases = toTableAliases;
                else
                    referencedAliases.AddRange(toTableAliases);
            }

            if (joins != null)
                joins.Add(this.Name, this);
        }

        /// <summary>
        ///   Left outer join'in "ON(...)" kýsmýnda yazýlan ifadeyi verir.</summary>
        public BaseCriteria OnCriteria
        {
            get
            {
                return onCriteria;
            }
        }

        /// <summary>
        ///   Left outer join'in "ON(...)" kýsmýnda yazýlan ifadeyi verir.</summary>
        public string OnCriteriaString
        {
            get
            {
                return onCriteriaString;
            }
        }

        /// <summary>
        ///   Left outer join'in "ON(...)" kýsmýnda yazýlan ifadedeki alias larýn listesini verir.</summary>
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