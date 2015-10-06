namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    ///   An object that is used to create criterias by employing operator overloading 
    ///   features of C# language, instead of using string based criterias.</summary>
    [Serializable]
    public class Criteria : BaseCriteria
    {
        public static readonly BaseCriteria Empty = new Criteria();
        public static readonly BaseCriteria False = new Criteria("0=1");
        public static readonly BaseCriteria True = new Criteria("1=1");
        
        private string expression;

        /// <summary>
        ///   Creates an empty criteria</summary>
        private Criteria()
        {
        }

        /// <summary>
        ///   Creates a new criteria with given condition. This condition is usually a 
        ///   field name, but it can also be a criteria text pre-generated.</summary>
        /// <remarks>
        ///   Usually used like: <c>new Criteria("fieldname") >= 5</c>.</remarks>
        /// <param name="text">
        ///   A field name or criteria condition (can be null)</param>
        public Criteria(string text)
        {
            this.expression = text;
        }

        /// <summary>
        ///   Creates a new criteria that contains field name of the metafield.</summary>
        /// <param name="field">
        ///   Field (required).</param>
        public Criteria(IField field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            this.expression = field.Expression;
        }

        /// <summary>
        ///   Belirtilen tablo alias'ı ve alan adını aralarına nokta koyarak içeren yeni bir 
        ///   kriter oluşturur.</summary>
        /// <param name="alias">
        ///   Tablo alias'ı. Null ya da boş olursa önemsenmez.</param>
        /// <param name="field">
        ///   Alan adı (zorunlu).</param>
        public Criteria(string alias, string field)
        {
            if (String.IsNullOrEmpty(field))
                throw new ArgumentNullException("field");

            if (String.IsNullOrEmpty(alias))
                throw new ArgumentNullException("alias");
            this.expression = alias + "." + field;
        }

        /// <summary>
        ///   Belirtilen numerik tablo alias'ı (başına T konarak) ve alan adını aralarına 
        ///   nokta koyarak içeren yeni bir kriter oluşturur.</summary>
        /// <param name="joinNumber">
        ///   Join numarası (T1 gibi kullanılır). Değer sıfırdan küçükse alan adı tek başına
        ///   kullanılır.</param>
        /// <param name="field">
        ///   Alan adı (zorunlu).</param>
        public Criteria(int joinNumber, string field)
        {
            if (String.IsNullOrEmpty(field))
                throw new ArgumentNullException("field");

            if (joinNumber < 0)
                throw new ArgumentOutOfRangeException("joinNumber");

            this.expression = joinNumber.TableAliasDot() + field;
        }

        /// <summary>
        ///   Belirtilen numerik tablo alias'ı (başına T konarak) ve alanın adını aralarına 
        ///   nokta koyarak içeren yeni bir kriter oluşturur.</summary>
        /// <param name="alias">
        ///   Join aliası (T1 gibi kullanılır)</param>
        /// <param name="field">
        ///   Alan nesnesi (zorunlu).</param>
        public Criteria(IAlias alias, IField field)
            : this(alias.Name, field.Name)
        {
        }

        /// <summary>
        ///   Belirtilen numerik tablo alias'ı (başına T konarak) ve alanın adını aralarına 
        ///   nokta koyarak içeren yeni bir kriter oluşturur.</summary>
        /// <param name="alias">
        ///   Join aliası (T1 gibi kullanılır)</param>
        /// <param name="field">
        ///   Alan nesnesi (zorunlu).</param>
        public Criteria(IAlias alias, string field)
            : this(alias.Name, field)
        {
        }

        /// <summary>
        ///   Belirtilen numerik tablo alias'ı (başına T konarak) ve alanın adını aralarına 
        ///   nokta koyarak içeren yeni bir kriter oluşturur.</summary>
        /// <param name="joinNumber">
        ///   Join numarası (T1 gibi kullanılır)</param>
        /// <param name="field">
        ///   Alan nesnesi (zorunlu).</param>
        public Criteria(int joinNumber, IField field)
            : this(joinNumber, field.Name)
        {
        }

        /// <summary>
        ///   Belirtilen join ve meta alanın adını aralarına nokta koyarak içeren yeni bir 
        ///   kriter oluşturur.</summary>
        /// <param name="join">
        ///   Tablo alias bilgisini içeren LeftJoin nesnesi (zorunlu).</param>
        /// <param name="field">
        ///   Field alan (zorunlu).</param>
        public Criteria(string join, IField field)
            : this(join, field.Name)
        {
        }

        /// <summary>
        ///   Belirtilen SqlQuery i içeren yeni bir 
        ///   kriter oluşturur.</summary>
        /// <param name="query">
        ///   Query nesnesi (genellikle sub query).</param>
        public Criteria(ISqlQuery query)
            : this(query.ToString())
        {
        }


        /// <summary>
        ///   Verilen alan adını köşeli parantez içine alarak yeni bir kriter oluşturur.
        ///   SQL'de boşluk içeren ya da keyword olan alan adlarının kullanılabilmesi 
        ///   için gerekebilir.</summary>
        /// <param name="fieldName">
        ///   Köşeli parantez içine alınıp kriterye çevrilecek alan adı (zorunlu).</param>
        /// <returns>
        ///   Alan adını köşeli parantez içinde içeren yeni bir kriter.</returns>
        public static Criteria Bracket(string fieldName)
        {
            if (String.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException("fieldName");

            return new Criteria("[" + fieldName + "]");
        }

        /// <summary>
        ///   Creates a new EXISTS criteria</summary>
        /// <param name="expression">
        ///   Expression</param>
        /// <returns></returns>
        public static BaseCriteria Exists(string expression)
        {
            return new UnaryCriteria(CriteriaOperator.Exists, new Criteria(expression));
        }

        /// <summary>
        ///   Gets if criteria is empty.</summary>
        public override bool IsEmpty
        {
            get 
            {
                return String.IsNullOrEmpty(expression);
            }
        }

        public override void ToString(StringBuilder sb, IQueryWithParams query)
        {
            sb.Append(this.expression);
        }

        public string Expression
        {
            get { return expression; }
        }
    }
}