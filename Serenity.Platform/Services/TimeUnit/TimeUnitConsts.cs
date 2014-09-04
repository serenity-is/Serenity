using System;

namespace Serenity.Data
{
    public static class TimeUnitConsts
    {
        public static int[] Multipliers = new int[] { 
            1,  
            60, 
            60 * 60,
            60 * 60 * 24,
            60 * 60 * 24 * 30,
            60 * 60 * 24 * 365
        };

        public static string ToAbbreviation(this TimeUnit unit)
        {
            return LocalText.Get("Enums.TimeUnit." + Enum.GetName(typeof(TimeUnit), unit) + "Abbr");
        }

        public static decimal SecondsToTimeUnitValue(int seconds, TimeUnit minUnit, out TimeUnit unit)
        {
            if (seconds != 0)
            {
                for (var u = TimeUnit.Years; u > minUnit; u--)
                {
                    decimal d = (decimal)seconds / (decimal)TimeUnitConsts.Multipliers[(int)u];
                    if (Math.Floor(d) == d ||
                        d - Math.Floor(d) == 0.5m)
                    {
                        unit = u;
                        return d;
                    }
                }
            }

            unit = minUnit;
            return Math.Round((decimal)seconds / (decimal)TimeUnitConsts.Multipliers[(int)minUnit] * 10) / 10m;
        }

        public static string SecondsToString(int seconds, TimeUnit minUnit)
        {
            if (seconds == 0)
                return "0";

            TimeUnit unit;
            var value = TimeUnitConsts.SecondsToTimeUnitValue(seconds, minUnit, out unit);
            var simge = TimeUnitConsts.ToAbbreviation(unit);
            return seconds.ToString(",0.#") + " " + simge;
        }
    }
}
