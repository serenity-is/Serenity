using System.Text;

namespace Serenity.Data
{
    public static class FormatHelper
    {
        public static string IntegerFormat(int digits,
            bool thousandSep = true, bool negatives = false, string symbol = null)
        {
            return DecimalFormat(digits, 0, 0, thousandSep, negatives, symbol);
        }

        public static string DecimalFormat(int digits, int decimals, int zeroDecimals = 0,
            bool thousandSep = true, bool negatives = false, string symbol = null)
        {
            StringBuilder format = new StringBuilder();

            if (digits > 15 || digits == 0)
                digits = 15;

            if (digits > 3 && thousandSep)
            {
                format.Append("#");
                format.Append(",");
                format.Append('#', digits - 2);
            }
            else
            {
                format.Append('#', digits - 1);
            }

            format.Append("0");

            if (decimals > 0)
            {
                if (decimals > 8)
                    decimals = 8;

                format.Append('.');

                if (zeroDecimals > 0)
                {
                    if (zeroDecimals > 8)
                        zeroDecimals = 8;

                    if (zeroDecimals > decimals)
                        zeroDecimals = decimals;

                    format.Append('0', zeroDecimals);
                    if (decimals > zeroDecimals)
                        format.Append('#', decimals - zeroDecimals);
                }
                else
                    format.Append('#', decimals);
            }

            if (!symbol.IsNullOrEmpty())
            {
                format.Append(' ');
                foreach (var c in symbol)
                {
                    format.Append('\\');
                    format.Append(c);
                }
            }

            if (!negatives)
                format.Append(';');

            return format.ToString();
        }

        public static string GetSymbolMask(string symbol)
        {
            symbol = symbol.TrimToEmpty();
            StringBuilder sb = new StringBuilder(symbol.Length * 2);
            sb.Append(symbol);
            for (var i = 0; i < symbol.Length; i++)
                sb.Insert(i * 2, '\\');
            return sb.ToString();
        }
    }

}
