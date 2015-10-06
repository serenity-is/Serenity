namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    [IgnoreNamespace]
    [Imported(ObeysTypeSystem = true)]
    [ScriptName("Array")]
    public class Criteria : BaseCriteria
    {
        public static BaseCriteria Empty
        {
            [InlineCode("['']")]
            get
            {
                return null;
            }
        }


        [InlineCode("[{text}]")]
        public Criteria(string text)
        {
        }

        [InlineCode("[({alias} + '.' + {field})]")]
        public Criteria(string alias, string field)
        {
        }

        [InlineCode("[({joinNumber} + '.' + {field})]")]
        public Criteria(int joinNumber, string field)
        {
        }

        [InlineCode("'[' + {fieldName} + ']'")]
        public static Criteria Bracket(string fieldName)
        {
            return null;
        }

        [InlineCode("['exists', [{expression}]]")]
        public static BaseCriteria Exists(string expression)
        {
            return null;
        }
    }
}