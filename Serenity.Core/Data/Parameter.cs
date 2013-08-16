using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serenity.Data
{
    public struct Parameter
    {
        private string _name;

        public Parameter(string name)
        {
            _name = name;
        }

        public Parameter(int index)
        {
            _name = index.IndexParam();
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
