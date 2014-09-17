
namespace BasicApplication.Northwind
{
    using Serenity;
    using Serenity.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using BasicApplication;

    public partial class EmployeeForm : PrefixedContext
    {
        public EmployeeForm(string idPrefix) : base(idPrefix) {}
    
        public StringEditor LastName { get { return ById<StringEditor>("LastName"); } }
        public StringEditor FirstName { get { return ById<StringEditor>("FirstName"); } }
        public StringEditor Title { get { return ById<StringEditor>("Title"); } }
        public StringEditor TitleOfCourtesy { get { return ById<StringEditor>("TitleOfCourtesy"); } }
        public DateEditor BirthDate { get { return ById<DateEditor>("BirthDate"); } }
        public DateEditor HireDate { get { return ById<DateEditor>("HireDate"); } }
        public StringEditor Address { get { return ById<StringEditor>("Address"); } }
        public StringEditor City { get { return ById<StringEditor>("City"); } }
        public StringEditor Region { get { return ById<StringEditor>("Region"); } }
        public StringEditor PostalCode { get { return ById<StringEditor>("PostalCode"); } }
        public StringEditor Country { get { return ById<StringEditor>("Country"); } }
        public StringEditor HomePhone { get { return ById<StringEditor>("HomePhone"); } }
        public StringEditor Extension { get { return ById<StringEditor>("Extension"); } }
        public StringEditor Photo { get { return ById<StringEditor>("Photo"); } }
        public StringEditor Notes { get { return ById<StringEditor>("Notes"); } }
        public IntegerEditor ReportsTo { get { return ById<IntegerEditor>("ReportsTo"); } }
        public StringEditor PhotoPath { get { return ById<StringEditor>("PhotoPath"); } }
    }
}

