using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   SQL sorgusundaki bir JOIN ifadesine karþýlýk gelir (INNER, OUTER, CROSS vs.)</summary>
    public abstract class Join : Alias
    {
        private RowFieldsBase fields;
        private string toTable;
        private BaseCriteria onCriteria;
        private string onCriteriaString;
        private HashSet<string> referencedAliases;

        public abstract string GetKeyword();

        protected Join(RowFieldsBase fields, string toTable, string alias, BaseCriteria onCriteria)
            : base(alias)
        {
            if (toTable == null)
                throw new ArgumentNullException("toTable");

            this.fields = fields;
            this.toTable = toTable;
            this.onCriteria = OnCriteria;

            if (!Object.ReferenceEquals(onCriteria, null))
            {
                this.onCriteriaString = this.onCriteria.ToString();

                var aliases = JoinAliasLocator.Locate(onCriteria.ToString());
                if (aliases != null && aliases.Count > 0)
                    referencedAliases = aliases;
            }

            var toTableAliases = JoinAliasLocator.Locate(ToTable);
            if (toTableAliases != null && toTableAliases.Count > 0)
            {
                if (referencedAliases == null)
                    referencedAliases = toTableAliases;
                else
                    referencedAliases.AddRange(toTableAliases);
            }

            fields._joins.Add(this.Name, this);
        }

        /// <summary>
        ///   Left outer join yapýlan tablo adýný verir.</summary>
        public string ToTable
        {
            get
            {
                return toTable;
            }
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

        public RowFieldsBase Fields
        {
            get { return fields; }
        }

    }
}