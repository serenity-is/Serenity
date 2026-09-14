using Microsoft.AspNetCore.Mvc;
using Serenity.Data.Mapping;

namespace ApplicationMetadataTest
{
    [TableName("[dbo].[AppMetaOrders]")]
    [Module("OrdersModule")]
    [LookupScript("AppMeta.Orders")]
    public class OrderRow : Row<OrderRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty]
        public int? OrderID
        {
            get { return fields.OrderID[this]; }
            set { fields.OrderID[this] = value; }
        }

        [NameProperty]
        [Size(50)]
        [NotNull]
        public string OrderName
        {
            get { return fields.OrderName[this]; }
            set { fields.OrderName[this] = value; }
        }

        [Column("[Meta Description]")]
        public string MetaDescription
        {
            get { return fields.MetaDescription[this]; }
            set { fields.MetaDescription[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field OrderID;
            public StringField OrderName;
            public StringField MetaDescription;
        }
    }

    [Route("~/AppMeta/Order/[action]")]
    public class OrderEndpoint : ServiceEndpoint
    {
        public ListResponse<OrderRow> List(IDbConnection connection, ListRequest request)
        {
            return null!;
        }
    }

    public class CustomerRow : Row<CustomerRow.RowFields>, IIdRow, INameRow
    {
        [IdProperty]
        public int? CustomerID
        {
            get { return fields.CustomerID[this]; }
            set { fields.CustomerID[this] = value; }
        }

        [NameProperty]
        public string CustomerName
        {
            get { return fields.CustomerName[this]; }
            set { fields.CustomerName[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field CustomerID;
            public StringField CustomerName;
        }
    }

    [Route("/AppMeta/Local/[action]")]
    public class LocalRowEndpoint : ServiceEndpoint
    {
        public ListResponse<LocalRow> List(IDbConnection connection, ListRequest request)
        {
            return null!;
        }
    }

    public class LocalRow : Row<LocalRow.RowFields>
    {
        public string LocalProp
        {
            get { return fields.LocalProp[this]; }
            set { fields.LocalProp[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public StringField LocalProp;
        }
    }

    [Route("~/AppMeta/NoAction")]
    public class NoActionEndpoint : ServiceEndpoint
    {
        public ListResponse<PlainRow> List(IDbConnection connection, ListRequest request)
        {
            return null!;
        }
    }

    public class PlainRow : Row<PlainRow.RowFields>
    {
        public string PlainProp
        {
            get { return fields.PlainProp[this]; }
            set { fields.PlainProp[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public StringField PlainProp;
        }
    }

    public class PropsRow : Row<PropsRow.RowFields>
    {
        [Origin("jLink", "OrigProp")]
        public string LinkedProp
        {
            get { return fields.LinkedProp[this]; }
            set { fields.LinkedProp[this] = value; }
        }

        [Expression("Props.foo")]
        public string ExprProp
        {
            get { return fields.ExprProp[this]; }
            set { fields.ExprProp[this] = value; }
        }

        public string PlainProp
        {
            get { return fields.PlainProp2[this]; }
            set { fields.PlainProp2[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public StringField LinkedProp;
            public StringField ExprProp;
            public StringField PlainProp2;
        }
    }

    public class AmbigRow : Row<AmbigRow.RowFields>
    {
        [Column("Same")]
        public string Alpha
        {
            get { return fields.Alpha[this]; }
            set { fields.Alpha[this] = value; }
        }

        [Column("Same")]
        public string Beta
        {
            get { return fields.Beta[this]; }
            set { fields.Beta[this] = value; }
        }

        public class RowFields : RowFieldsBase
        {
            public StringField Alpha;
            public StringField Beta;
        }
    }
}
