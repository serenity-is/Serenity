using System;
using System.Collections.Generic;
using System.Text;

namespace Serenity.Data
{
    /// <summary>
    ///   Used to define aliases like (T0).</summary>
    public class Alias
    {
        public static readonly Alias T0 = new Alias(0);
        public static readonly Alias T1 = new Alias(1);
        public static readonly Alias T2 = new Alias(2);
        public static readonly Alias T3 = new Alias(3);
        public static readonly Alias T4 = new Alias(4);
        public static readonly Alias T5 = new Alias(5);
        public static readonly Alias T6 = new Alias(6);
        public static readonly Alias T7 = new Alias(7);
        public static readonly Alias T8 = new Alias(8);
        public static readonly Alias T9 = new Alias(9);

        private string alias;
        private string aliasDot;

        public Alias(int alias)
        {
            this.alias = alias.TableAlias();
            this.aliasDot = alias.TableAliasDot();
        }

        public Alias(string alias)
        {
            if (alias == null)
                throw new ArgumentNullException("alias");

            this.alias = alias;
            this.aliasDot = alias + ".";
        }

        public string Name
        {
            get { return alias; }
        }

        public string this[string fieldName]
        {
            get { return this.aliasDot + fieldName; }
        }

        public string this[Field field]
        {
            get { return this.aliasDot + field.Name; }
        }
    }
}