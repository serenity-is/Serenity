using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serenity
{
    public static class StringHelpers
    {
        public const string NbspSlash = " - ";

        public static string ParseNbspSlashedKod(string kodIsim)
        {
            if (kodIsim.IsNullOrEmpty())
                return String.Empty;

            int index = kodIsim.IndexOf(NbspSlash);
            if (index >= 0)
                return kodIsim.Substring(0, index);
            else
                return kodIsim;
        }

        public static string PutNbspSlash(string kod, string isim, bool onlyIfNonEmpty = false)
        {
            if (!onlyIfNonEmpty || ((!kod.IsNullOrEmpty()) && (!isim.IsNullOrEmpty())))
                return kod + NbspSlash + isim;
            else if (kod.IsNullOrEmpty())
                return isim ?? "";
            else
                return kod ?? "";
        }
    }
}
