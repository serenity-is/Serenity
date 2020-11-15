
namespace SerenityIs.Commerce.Entities
{
    using Serenity.ComponentModel;
    using Serenity.Data;
    using Serenity.Data.Mapping;
    using System;
    using System.ComponentModel;

    [ConnectionKey("Default"), TableName("[ecm].[Orders]"), DisplayName("Order"), InstanceName("Order"), TwoLevelCached]
    [ReadPermission("Administration:General")]
    [ModifyPermission("Administration:General")]
    [LookupScript("Commerce.Order")]
    public sealed class OrderRow : Row<OrderRow.RowFields>, IIdRow, INameRow, IIsActiveDeletedRow
    {
        [DisplayName("Order Id"), Identity]
        public int? OrderId
        {
            get { return Fields.OrderId[this]; }
            set { Fields.OrderId[this] = value; }
        }

        [DisplayName("Order Date"), NotNull, DateTimeKind(DateTimeKind.Utc), DateTimeEditor, QuickFilter, Updatable(false)]
        public DateTime? OrderDate
        {
            get { return Fields.OrderDate[this]; }
            set { Fields.OrderDate[this] = value; }
        }

        [DisplayName("Product"), ForeignKey("[ecm].[Products]", "ProductId"), LeftJoin("jProduct"), TextualField("ProductProductName")]
        public Int32? ProductId
        {
            get { return Fields.ProductId[this]; }
            set { Fields.ProductId[this] = value; }
        }

        [DisplayName("Product Name"), Size(200), QuickSearch]
        public String ProductName
        {
            get { return Fields.ProductName[this]; }
            set { Fields.ProductName[this] = value; }
        }

        [DisplayName("Unit Price"), Size(18), Scale(2), NotNull]
        public Decimal? UnitPrice
        {
            get { return Fields.UnitPrice[this]; }
            set { Fields.UnitPrice[this] = value; }
        }

        [DisplayName("Duration")]
        public Int32? Duration
        {
            get { return Fields.Duration[this]; }
            set { Fields.Duration[this] = value; }
        }

        [DisplayName("Quantity"), Size(18), Scale(2), NotNull]
        public Decimal? Quantity
        {
            get { return Fields.Quantity[this]; }
            set { Fields.Quantity[this] = value; }
        }

        [DisplayName("Line Total"), Size(18), Scale(2), NotNull]
        public Decimal? LineTotal
        {
            get { return Fields.LineTotal[this]; }
            set { Fields.LineTotal[this] = value; }
        }

        [DisplayName("First Name"), Size(50), QuickSearch]
        public String FirstName
        {
            get { return Fields.FirstName[this]; }
            set { Fields.FirstName[this] = value; }
        }

        [DisplayName("Surname"), Size(50), QuickSearch]
        public String Surname
        {
            get { return Fields.Surname[this]; }
            set { Fields.Surname[this] = value; }
        }

        [DisplayName("Company Name"), Size(100), QuickSearch]
        public String CompanyName
        {
            get { return Fields.CompanyName[this]; }
            set { Fields.CompanyName[this] = value; }
        }

        [DisplayName("Address"), Size(400), TextAreaEditor(Rows = 2)]
        public String Address
        {
            get { return Fields.Address[this]; }
            set { Fields.Address[this] = value; }
        }

        [DisplayName("Postcode"), Size(10)]
        public String Postcode
        {
            get { return Fields.Postcode[this]; }
            set { Fields.Postcode[this] = value; }
        }

        [DisplayName("Country"), Size(100), QuickSearch]
        public String Country
        {
            get { return Fields.Country[this]; }
            set { Fields.Country[this] = value; }
        }

        [DisplayName("Email"), Size(50), QuickSearch]
        public String Email
        {
            get { return Fields.Email[this]; }
            set { Fields.Email[this] = value; }
        }

        [DisplayName("Phone"), Size(50)]
        public String Phone
        {
            get { return Fields.Phone[this]; }
            set { Fields.Phone[this] = value; }
        }

        [DisplayName("Git Hub Usernames"), Size(200), QuickSearch]
        public String GitHubUsernames
        {
            get { return Fields.GitHubUsernames[this]; }
            set { Fields.GitHubUsernames[this] = value; }
        }

        [DisplayName("Git Hub Access Granted"), NotNull]
        public Boolean? GitHubAccessGranted
        {
            get { return Fields.GitHubAccessGranted[this]; }
            set { Fields.GitHubAccessGranted[this] = value; }
        }

        [DisplayName("Customer Notes"), TextAreaEditor(Rows = 2)]
        public String CustomerNotes
        {
            get { return Fields.CustomerNotes[this]; }
            set { Fields.CustomerNotes[this] = value; }
        }

        [DisplayName("Seller Notes"), TextAreaEditor(Rows = 2)]
        public String SellerNotes
        {
            get { return Fields.SellerNotes[this]; }
            set { Fields.SellerNotes[this] = value; }
        }

        [DisplayName("Invoice Number"), Size(20)]
        public String InvoiceNumber
        {
            get { return Fields.InvoiceNumber[this]; }
            set { Fields.InvoiceNumber[this] = value; }
        }

        [DisplayName("Cari ID")]
        public Int32? CariId
        {
            get { return Fields.CariId[this]; }
            set { Fields.CariId[this] = value; }
        }

        [DisplayName("Fatura ID")]
        public Int32? FaturaId
        {
            get { return Fields.FaturaId[this]; }
            set { Fields.FaturaId[this] = value; }
        }

        [DisplayName("Stok ID")]
        public Int32? StokId
        {
            get { return Fields.StokId[this]; }
            set { Fields.StokId[this] = value; }
        }

        [DisplayName("Payment Guid"), NotNull, Insertable(false), Updatable(false)]
        public Guid? PaymentGuid
        {
            get { return Fields.PaymentGuid[this]; }
            set { Fields.PaymentGuid[this] = value; }
        }

        [Expression("(COALESCE (CAST(T0.OrderId AS VARCHAR(10)), '') + " +
            "COALESCE(' - ' + FORMAT(T0.OrderDate, 'yyyy/MM/dd'), '') + " +
            "COALESCE(' - ' + T0.ProductName, '') + " +
            "COALESCE(' - ' + T0.FirstName, '') + " +
            "COALESCE(' ' + T0.Surname, '') + " +
            "COALESCE(' - ' + T0.CompanyName, ''))")]
        public String OrderDisplay
        {
            get { return Fields.OrderDisplay[this]; }
            set { Fields.OrderDisplay[this] = value; }
        }

        [DisplayName("Payment Transaction Number"), Size(100)]
        public String PaymentTransactionNumber
        {
            get { return Fields.PaymentTransactionNumber[this]; }
            set { Fields.PaymentTransactionNumber[this] = value; }
        }

        [DisplayName("Payment Status Message"), Size(-1), Updatable(false)]
        public String PaymentStatusMessage
        {
            get { return Fields.PaymentStatusMessage[this]; }
            set { Fields.PaymentStatusMessage[this] = value; }
        }

        [DisplayName("Delivery Date"), DateTimeKind(DateTimeKind.Utc), DateTimeEditor(IntervalMinutes = 1)]
        public DateTime? DeliveryDate
        {
            get { return Fields.DeliveryDate[this]; }
            set { Fields.DeliveryDate[this] = value; }
        }

        [DisplayName("Expiration Date"), DateTimeKind(DateTimeKind.Utc), DateTimeEditor(IntervalMinutes = 1)]
        public DateTime? Expiration
        {
            get { return Fields.Expiration[this]; }
            set { Fields.Expiration[this] = value; }
        }

		[DisplayName("Has Expired"), Expression("(CASE WHEN t0.Expiration < getdate() THEN CAST (1 as bit) ELSE CAST (0 as bit) END)")]
		public Boolean? HasExpired
		{
			get { return Fields.HasExpired[this]; }
			set { Fields.HasExpired[this] = value; }
		}

        [DisplayName("Product Name"), Expression("jProduct.[ProductName]"), QuickFilter]
        public String ProductProductName
        {
            get { return Fields.ProductProductName[this]; }
            set { Fields.ProductProductName[this] = value; }
        }

        [DisplayName("Product Unit Price"), Expression("jProduct.[UnitPrice]")]
        public Decimal? ProductUnitPrice
        {
            get { return Fields.ProductUnitPrice[this]; }
            set { Fields.ProductUnitPrice[this] = value; }
        }

        [DisplayName("Product Currency Type"), Expression("jProduct.[CurrencyType]")]
        public Int32? ProductCurrencyType
        {
            get { return Fields.ProductCurrencyType[this]; }
            set { Fields.ProductCurrencyType[this] = value; }
        }

        [DisplayName("Product Duration"), Expression("jProduct.[Duration]")]
        public Int32? ProductDuration
        {
            get { return Fields.ProductDuration[this]; }
            set { Fields.ProductDuration[this] = value; }
        }

        [DisplayName("Product Duration Unit"), Expression("jProduct.[DurationUnit]")]
        public Int32? ProductDurationUnit
        {
            get { return Fields.ProductDurationUnit[this]; }
            set { Fields.ProductDurationUnit[this] = value; }
        }

        [DisplayName("Product Purchase Info"), Expression("jProduct.[PurchaseInfo]")]
        public String ProductPurchaseInfo
        {
            get { return Fields.ProductPurchaseInfo[this]; }
            set { Fields.ProductPurchaseInfo[this] = value; }
        }

        [DisplayName("Used Promotion Code")]
        public Int32? PromotionCodeId
        {
            get { return Fields.PromotionCodeId[this]; }
            set { Fields.PromotionCodeId[this] = value; }
        }

		[DisplayName("Renewal Offer Code")]
		public Int32? RenewalCodeId
		{
			get { return Fields.RenewalCodeId[this]; }
			set { Fields.RenewalCodeId[this] = value; }
		}

		[DisplayName("Renewal Offer Mail Sent On"), DateTimeKind(DateTimeKind.Utc), DateTimeEditor]
		public DateTime? RenewalOfferOn
		{
			get { return Fields.RenewalOfferOn[this]; }
			set { Fields.RenewalOfferOn[this] = value; }
		}

		[DisplayName("Renewal Offer Sent"), Expression("(CASE WHEN t0.RenewalOfferOn IS NOT NULL THEN CAST (1 as bit) ELSE CAST (0 as bit) END)")]
		public Boolean? RenewalOfferSent
		{
			get { return Fields.RenewalOfferSent[this]; }
			set { Fields.RenewalOfferSent[this] = value; }
		}

		[DisplayName("Serenity.is Username")]
        public Int32? UserId
        {
            get { return Fields.UserId[this]; }
            set { Fields.UserId[this] = value; }
        }

        [DisplayName("Tax Number"), Size(50)]
        public string TaxNumber
        {
            get { return Fields.TaxNumber[this]; }
            set { Fields.TaxNumber[this] = value; }
        }

        public Int16? IsActive
        {
            get { return Fields.IsActive[this]; }
            set { Fields.IsActive[this] = value; }
        }

        IIdField IIdRow.IdField => Fields.OrderId;
        StringField INameRow.NameField => Fields.OrderDisplay; 
        Int16Field IIsActiveRow.IsActiveField => Fields.IsActive;

        public OrderRow(RowFields fields = null)
            : base(fields)
        {
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field OrderId;
            public DateTimeField OrderDate;
            public Int32Field ProductId;
            public StringField ProductName;
            public DecimalField UnitPrice;
            public Int32Field CurrencyType;
            public Int32Field Duration;
            public DecimalField Quantity;
            public DecimalField LineTotal;
            public StringField FirstName;
            public StringField Surname;
            public StringField CompanyName;
            public StringField Address;
            public StringField Postcode;
            public StringField Country;
            public StringField Email;
            public StringField Phone;
            public StringField GitHubUsernames;
            public BooleanField GitHubAccessGranted;
            public StringField CustomerNotes;
            public StringField SellerNotes;
            public StringField InvoiceNumber;
            public GuidField PaymentGuid;
            public Int32Field PaymentProviderType;
            public StringField PaymentTransactionNumber;
            public Int32Field PaymentStatus;
            public StringField PaymentStatusMessage;
            public DateTimeField DeliveryDate;
            public DateTimeField Expiration;
            public Int32Field PromotionCodeId;
			public Int32Field RenewalCodeId;
			public DateTimeField RenewalOfferOn;
            public StringField OrderDisplay;
            public StringField TaxNumber;

            public StringField ProductProductName;
            public DecimalField ProductUnitPrice;
            public Int32Field ProductCurrencyType;
            public Int32Field ProductDuration;
            public Int32Field ProductDurationUnit;
            public StringField ProductPurchaseInfo;

            public Int32Field CariId;
            public Int32Field StokId;
            public Int32Field FaturaId;
            public Int32Field UserId;
            public Int16Field IsActive;

			public BooleanField HasExpired;
			public BooleanField RenewalOfferSent;

            public RowFields()
            {
                LocalTextPrefix = "Commerce.Order";
            }
        }
    }
}


