using Serenity.Data;
using System;

namespace Serenity.Services
{
    internal class BracketRemoverDialect : ISqlDialect
    {
        public static readonly BracketRemoverDialect Instance = new BracketRemoverDialect();

        public bool CanUseOffsetFetch
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool CanUseRowNumber
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool CanUseSkipKeyword
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public char CloseQuote
        {
            get
            {
                return '\x1';
            }
        }

        public string ConcatOperator
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string DateFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string DateTimeFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsLikeCaseSensitive
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool MultipleResultsets
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool NeedsBoolWorkaround
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool NeedsExecuteBlockStatement
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string OffsetFetchFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string OffsetFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public char OpenQuote
        {
            get
            {
                return '\x1';
            }
        }

        public char ParameterPrefix
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ScopeIdentityExpression
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ServerType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string SkipKeyword
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string TakeKeyword
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string TimeFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool UseDateTime2
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool UseReturningIdentity
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool UseReturningIntoVar
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool UseRowNum
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool UseScopeIdentity
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool UseTakeAtEnd
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string QuoteColumnAlias(string s)
        {
            return s;
        }

        public string QuoteIdentifier(string s)
        {
            return s;
        }

        public string QuoteUnicodeString(string s)
        {
            throw new NotImplementedException();
        }
    }
}