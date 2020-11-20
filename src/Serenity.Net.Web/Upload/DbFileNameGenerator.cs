using Serenity.IO;
using System;
using System.IO;

namespace Serenity.Web
{
    public class DbFileNameGenerator
    {
        private readonly string format;

        public DbFileNameGenerator(string format)
        {
            this.format = format ?? throw new ArgumentOutOfRangeException(nameof(format));
        }

        public string Format(object identity, string originalName, object entity)
        {
            long l;
            object groupKey;
            string s;
            if (identity == null)
                groupKey = "_";
            else if (identity is Guid)
            {
                s = ((Guid)identity).ToString("N");
                identity = s;
                groupKey = s.Substring(0, 2);
            }
            else
            {
                s = identity.ToString();
                if (long.TryParse(s, out l))
                    groupKey = l / 1000;
                else if (s.Length == 0)
                    groupKey = "_";
                else
                    groupKey = s.SafeSubstring(0, 2);
            }

            var formatted = string.Format(format, identity, groupKey, TemporaryFileHelper.RandomFileCode(), DateTime.Now,
                Path.GetFileName(originalName));
        }
    }
}