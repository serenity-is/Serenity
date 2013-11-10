using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   SQL sorgusundaki bir LEFT OUTER JOIN ile ilgili bilgileri tutan Field sýnýf.</summary>
    public class LeftJoin : Alias
    {
        private RowFieldsBase fields;
        private string toTable;
        private string onCriteria;
        private HashSet<string> onCriteriaAliases;

        /// <summary>
        ///   Verilen tablo adý, alias ve baðlantý koþuluna sahip yeni bir LeftJoin oluþturur.</summary>
        /// <param name="toTable">
        ///   Sorguya left outer join ile dahil edilen tablo adý (zorunlu).</param>
        /// <param name="alias">
        ///   Sorguya left outer join ile dahil edilen tabloya atanan alias (zorunlu).</param>
        /// <param name="onCriteria">
        ///   Left outer join iþleminin "ON(...)" kýsmýnda belirtilen ifade (zorunlu).</param>
        public LeftJoin(RowFieldsBase fields, string toTable, string alias, string onCriteria)
            : base(alias)
        {
            if (toTable == null)
                throw new ArgumentNullException("toTable");

            if (onCriteria == null)
                throw new ArgumentNullException("onCriteria");

            this.fields = fields;
            this.toTable = toTable;
            this.onCriteria = onCriteria.TrimToNull();

            if (onCriteria != null)
            {
                var aliases = JoinAliasLocator.Locate(onCriteria);
                if (aliases.Count > 0)
                    onCriteriaAliases = aliases;
            }

            fields._leftJoins.Add(this.Name, this);
        }

        /// <summary>
        ///   Verilen tablo adý, alias ve baðlantý koþulu filtresine sahip yeni bir 
        ///   LeftJoin oluþturur.</summary>
        /// <param name="toTable">
        ///   Sorguya left outer join ile dahil edilen tablo adý (zorunlu).</param>
        /// <param name="alias">
        ///   Sorguya left outer join ile dahil edilen tabloya atanan alias (zorunlu).</param>
        /// <param name="onCriteria">
        ///   Left outer join iþleminin "ON(...)" kýsmýna karþýlýk gelen filtre nesnesi (zorunlu).</param>
        public LeftJoin(RowFieldsBase fields, string toTable, string alias, BaseCriteria onCriteria)
            : this(fields, toTable, alias, onCriteria.ToStringCheckNoParams())
        {
        }

        /// <summary>
        ///   Verilen tablo adý, join indeksi ve baðlantý koþuluna sahip yeni bir 
        ///   LeftJoin oluþturur.</summary>
        /// <param name="toTable">
        ///   Sorguya left outer join ile dahil edilen tablo adý (zorunlu).</param>
        /// <param name="alias">
        ///   Sorguya left outer join ile dahil edilen tabloya atanacak join indeksi. Ör. "1" verilirse join
        ///   alias "T1" olur.</param>
        /// <param name="onCriteria">
        ///   Left outer join iþleminin "ON(...)" kýsmýna karþýlýk gelen filtre nesnesi (zorunlu).</param>
        public LeftJoin(RowFieldsBase fields, string toTable, int alias, BaseCriteria onCriteria)
            : this(fields, toTable, alias.TableAlias(), onCriteria.ToStringCheckNoParams())
        {
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
        public string OnCriteria
        {
            get
            {
                return onCriteria;
            }
        }


        /// <summary>
        ///   Left outer join'in "ON(...)" kýsmýnda yazýlan ifadedeki alias larýn listesini verir.</summary>
        public HashSet<string> OnCriteriaAliases
        {
            get
            {
                return onCriteriaAliases;
            }
        }

        public RowFieldsBase Fields
        {
            get { return fields; }
        }

    }
}