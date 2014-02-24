namespace Samples
{
    using Serenity;
    using Serenity.Data;

    public partial class SqlQuerySamples
    {
        public class PeopleAlias : Alias
        {
            public PeopleAlias(string alias) 
                : base(alias) 
            { 
            }

            public string ID { get { return this["ID"]; } }
            public string Firstname { get { return this["Firstname"]; } }
            public string Surname { get { return this["Surname"]; } }
            public string Age { get { return this["Age"]; } }
        }

        public class CityAlias : Alias
        {
            public CityAlias(string alias)
                : base(alias)
            {
            }

            public string ID { get { return this["ID"]; } }
            public string CountryID { get { return this["CountryID"]; } }
            public string CityName { get { return this["CityName"]; } }
        }

        public class CountryAlias : Alias
        {
            public CountryAlias(string alias)
                : base(alias)
            {
            }

            public string ID { get { return this["ID"]; } }
            public string CountryName { get { return this["CountryName"]; } }
        }

        public static string UsingPredefinedClasses()
        {
            var p = new PeopleAlias("p");
            var c = new CityAlias("c");
            var o = new CountryAlias("o");

            return new SqlQuery()
                .Select(p.Firstname)
                .Select(p.Surname)
                .Select(c.CityName)
                .Select(o.CountryName)
                .From(p)
                .From(c)
                .From(o)
                .OrderBy(p.Age)
                .ToString();
        }
    }
}