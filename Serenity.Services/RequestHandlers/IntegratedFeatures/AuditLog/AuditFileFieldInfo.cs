using System.Collections.Generic;
using Serenity.Data;
using System;

namespace Serenity.Services
{
    public class AuditFileFieldInfo
    {
        public AuditFileFieldInfo(StringField filename, StringField originalName, Int32Field size)
        {
            if (ReferenceEquals(null, filename))
                throw new ArgumentNullException("filenameField");

            Filename = filename;
            OriginalName = originalName;
            Size = size;
        }

        public StringField Filename { get; private set; }
        public StringField OriginalName { get; private set; }
        public Int32Field Size { get; private set; }
    }
}