using System;
using Serenity.Data;
using Newtonsoft.Json;

namespace Serenity.Data
{
    using R = DisplayOrderRow;

    ///   Entity class for a "DisplayOrders" table</summary>
    [JsonConverter(typeof(JsonRowConverter))]
    public sealed class DisplayOrderRow : Row, IIdRow, IIsActiveRow, IDisplayOrderRow
    {
        /// <summary>Table name</summary>
        public const string TableName = "DisplayOrders";

        public class RowFields : RowFieldsBase
        {
            /// <summary>ID field</summary>
            public readonly Int32Field ID;
            /// <summary>GroupID field</summary>
            public readonly Int32Field GroupID;
            /// <summary>DisplayOrder field</summary>
            public readonly Int32Field DisplayOrder;
            /// <summary>IsActive field</summary>
            public readonly Int16Field IsActive;

            public RowFields()
                : base(R.TableName, "")
            {
                ID = new Int32Field(this, "ID", "ID", 0, FieldFlags.Identity,
                    r => ((R)r)._ID, (r, v) => ((R)r)._ID = v);

                GroupID = new Int32Field(this, "GroupID", "Değer No", 0, FieldFlags.Required,
                    r => ((R)r)._GroupID, (r, v) => ((R)r)._GroupID = v);

                DisplayOrder = new Int32Field(this, "DisplayOrder", "Gösterim Sırası", 0, FieldFlags.Required,
                    r => ((R)r)._DisplayOrder, (r, v) => ((R)r)._DisplayOrder = v);

                IsActive = new Int16Field(this, "IsActive", "Aktif", 0, FieldFlags.Required,
                    r => ((R)r)._IsActive, (r, v) => ((R)r)._IsActive = v);
            }
        }

        /// <summary>Row fields</summary>
        public static readonly RowFields Fields;
        /// <summary>Shared instance of the row class</summary>
        public static readonly DisplayOrderRow Instance;

        /// <summary>Initializes field definitions</summary>
        static DisplayOrderRow()
        {
            Fields = new RowFields();
            Instance = new DisplayOrderRow();
        }

        private Int32? _ID;
        private Int32? _GroupID;
        private Int32? _DisplayOrder;
        private Int16? _IsActive;

        /// <summary>Creates a new DisplayOrderRow instance.</summary>
        public DisplayOrderRow()
            : base(Fields)
        {
        }

        /// <summary>Creates a new DisplayOrderRow instance.</summary>
        public override Row CreateNew()
        {
            return new DisplayOrderRow();
        }

        /// <summary>ID field value</summary>
        public Int32? ID
        {
            get { return _ID; }
            set { Fields.ID[this] = value; }
        }

        /// <summary>GroupID field value</summary>
        public Int32? GroupID
        {
            get { return _GroupID; }
            set { Fields.GroupID[this] = value; }
        }

        /// <summary>DisplayOrder field value</summary>
        public Int32? DisplayOrder
        {
            get { return _DisplayOrder; }
            set { Fields.DisplayOrder[this] = value; }
        }

        /// <summary>IsActive field value</summary>
        public Int16? IsActive
        {
            get { return _IsActive; }
            set { Fields.IsActive[this] = value; }
        }

        /// <summary>Implements IIdRow.IdField by returning the identity field</summary>
        IIdField IIdRow.IdField
        {
            get { return Fields.ID; }
        }

        Int16Field IIsActiveRow.IsActiveField
        {
            get { return Fields.IsActive; }
        }

        public Int32Field DisplayOrderField
        {
            get { return Fields.DisplayOrder; }
        }
    }
}