using System;

namespace Serenity.Data
{
    /// <summary>
    ///   SQL sorgusundaki bir LEFT OUTER JOIN ile ilgili bilgileri tutan Field sýnýf.</summary>
    public class LeftJoin
    {
        /// <summary>Sorguya left outer join ile dahil edilen tablo adý (zorunlu).</summary>
        private string _joinTable;
        /// <summary>Sorguya left outer join ile dahil edilen tabloya atanan alias (zorunlu).</summary>
        private string _joinAlias;
        /// <summary>Left outer join iþleminin "ON (...)" kýsmýnda belirtilen ifade (zorunlu).</summary>
        private string _joinCondition;

        private string _sourceAlias;
        private string _sourceKeyField;
        private string _joinKeyField;

        /// <summary>
        ///   Verilen tablo adý, alias ve baðlantý koþuluna sahip yeni bir LeftJoin oluþturur.</summary>
        /// <param name="joinTable">
        ///   Sorguya left outer join ile dahil edilen tablo adý (zorunlu).</param>
        /// <param name="joinAlias">
        ///   Sorguya left outer join ile dahil edilen tabloya atanan alias (zorunlu).</param>
        /// <param name="joinCondition">
        ///   Left outer join iþleminin "ON(...)" kýsmýnda belirtilen ifade (zorunlu).</param>
        public LeftJoin(RowFieldsBase fields, string joinTable, string joinAlias, string joinCondition)
        {
            if (joinTable == null)
                throw new ArgumentNullException("joinTable");
            if (joinAlias == null)
                throw new ArgumentNullException("joinAlias");           
            if (joinCondition == null)
                throw new ArgumentNullException("joinCondition");

            _joinTable = joinTable;
            _joinAlias = joinAlias;
            _joinCondition = joinCondition;

            if (IsValidIdentifier(_joinAlias))
            {
                var c = _joinCondition.Split(new char[] { '=' });
                if (c.Length == 2)
                {
                    var p1 = c[0].Trim().Split(new char[] { '.' });
                    var p2 = c[1].Trim().Split(new char[] { '.' });
                    if (p1.Length == 2 &&
                        p2.Length == 2 &&
                        IsValidIdentifier(p1[0].Trim()) &&
                        IsValidIdentifier(p1[1].Trim()) &&
                        IsValidIdentifier(p2[0].Trim()) &&
                        IsValidIdentifier(p2[1].Trim()) &&
                        String.Compare(p1[0].Trim(), p2[0].Trim(), StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        if (String.Compare(p1[0].Trim(), _joinAlias, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            _joinKeyField = p1[1].Trim();
                            _sourceAlias = p2[0].Trim();
                            _sourceKeyField = p2[1].Trim();
                        }
                        else if (String.Compare(p2[0].Trim(), _joinAlias, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            _joinKeyField = p2[1].Trim();
                            _sourceAlias = p1[0].Trim();
                            _sourceKeyField = p1[1].Trim();
                        }
                    }
                }
            }

            fields._leftJoins.Add(_joinAlias, this);
        }

        public static bool IsValidIdentifier(string s)
        {
            if (s == null || s.Length == 0)
                return false;

            var c = Char.ToUpperInvariant(s[0]);
            if (c != '_' && (c < 'A' || c > 'Z'))
                return false;

            for (var i = 1; i < s.Length; i++)
            {
                c = Char.ToUpperInvariant(s[i]);
                if (c != '_' &&
                    !((c >= '0' && c <= '9') ||
                      (c >= 'A' && c <= 'Z')))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///   Verilen tablo adý, alias ve baðlantý koþulu filtresine sahip yeni bir 
        ///   LeftJoin oluþturur.</summary>
        /// <param name="joinTable">
        ///   Sorguya left outer join ile dahil edilen tablo adý (zorunlu).</param>
        /// <param name="joinAlias">
        ///   Sorguya left outer join ile dahil edilen tabloya atanan alias (zorunlu).</param>
        /// <param name="joinCondition">
        ///   Left outer join iþleminin "ON(...)" kýsmýna karþýlýk gelen filtre nesnesi (zorunlu).</param>
        public LeftJoin(RowFieldsBase fields, string joinTable, string joinAlias, Criteria joinCondition)
            : this(fields, joinTable, joinAlias, joinCondition.ToString())
        {
        }

        /// <summary>
        ///   Verilen tablo adý, join indeksi ve baðlantý koþuluna sahip yeni bir 
        ///   LeftJoin oluþturur.</summary>
        /// <param name="joinTable">
        ///   Sorguya left outer join ile dahil edilen tablo adý (zorunlu).</param>
        /// <param name="joinNumber">
        ///   Sorguya left outer join ile dahil edilen tabloya atanacak join indeksi. Ör. "1" verilirse join
        ///   alias "T1" olur.</param>
        /// <param name="joinCondition">
        ///   Left outer join iþleminin "ON(...)" kýsmýna karþýlýk gelen filtre nesnesi (zorunlu).</param>
        public LeftJoin(RowFieldsBase fields, string joinTable, int joinNumber, Criteria joinCondition)
            : this(fields, joinTable, joinNumber.TableAlias(), joinCondition.ToString())
        {
        }

        //public FieldMeta Foreign(string joinField, string name)
        //{
        //    var meta = new FieldMeta(name, FieldFlags.Foreign) { Expression = this.JoinAlias + "." + joinField };
        //    return meta;
        //}

        //public FieldMeta Foreign(string joinField)
        //{
        //    var meta = new FieldMeta(joinField, FieldFlags.Foreign) { Expression = this.JoinAlias + "." + joinField };
        //    return meta;
        //}

        /// <summary>
        ///   Left outer join yapýlan tablo adýný verir.</summary>
        public string JoinTable
        {
            get
            {
                return _joinTable;
            }
        }

        /// <summary>
        ///   Left outer join yapýlan tabloya atanan alias'ý verir.</summary>
        public string JoinAlias
        {
            get
            {
                return _joinAlias;
            }
        }

        /// <summary>
        ///   Left outer join'in "ON(...)" kýsmýnda yazýlan ifadeyi verir.</summary>
        public string JoinCondition
        {
            get
            {
                return _joinCondition;
            }
        }

        public string SourceAlias
        {
            get
            {
                return _sourceAlias;
            }
        }

        public string SourceKeyField
        {
            get
            {
                return _sourceKeyField;
            }
        }

        public string JoinKeyField
        {
            get
            {
                return _joinKeyField;
            }
        }

        public string this[string field]
        {
            get { return this.JoinAlias + "." + field; }
        }

        public string this[Field field]
        {
            get { return this.JoinAlias + "." + field.Name; }
        }
    }
}