using System;
using Serenity.Data;
using Newtonsoft.Json;

namespace Serenity.Test.Data
{
    using R = FakeDisplayOrderRow;

    ///   Entity class for a "DisplayOrders" table with fake ID field (Group ID instead of ID)</summary>
    public sealed class FakeDisplayOrderRow : Row, IIdRow, IIsActiveRow, IDisplayOrderRow
    {
        /// <summary>Table name</summary>
        public const string TableName = "DisplayOrders";

        public class RowFields : RowFieldsBase
        {
            /// <summary>ID field</summary>
            public Int32Field ID;
            /// <summary>GroupID field</summary>
            public Int32Field GroupID;
            /// <summary>DisplayOrder field</summary>
            public Int32Field DisplayOrder;
            /// <summary>IsActive field</summary>
            public Int16Field IsActive;

            public RowFields()
                : base(R.TableName, "")
            {
                ID = new Int32Field(this, "ID", "ID", 0, FieldFlags.NotNull,
                    r => ((R)r)._id, (r, v) => ((R)r)._id = v);

                GroupID = new Int32Field(this, "GroupID", "Değer No", 0, FieldFlags.Identity,
                    r => ((R)r)._groupId, (r, v) => ((R)r)._groupId = v);

                DisplayOrder = new Int32Field(this, "DisplayOrder", "Gösterim Sırası", 0, FieldFlags.Required,
                    r => ((R)r)._displayOrder, (r, v) => ((R)r)._displayOrder = v);

                IsActive = new Int16Field(this, "IsActive", "Aktif", 0, FieldFlags.Required,
                    r => ((R)r)._isActive, (r, v) => ((R)r)._isActive = v);
            }
        }

        /// <summary>Row fields</summary>
        public static readonly RowFields Fields;
        /// <summary>Shared instance of the row class</summary>
        public static readonly FakeDisplayOrderRow Instance;

        /// <summary>Initializes field definitions</summary>
        static FakeDisplayOrderRow()
        {
            Fields = new RowFields();
            Instance = new FakeDisplayOrderRow();
        }

        private Int32? _id;
        private Int32? _groupId;
        private Int32? _displayOrder;
        private Int16? _isActive;

        /// <summary>Creates a new FakeDisplayOrderRow instance.</summary>
        public FakeDisplayOrderRow()
            : base(Fields)
        {
        }

        /// <summary>Creates a new FakeDisplayOrderRow instance.</summary>
        public override Row CreateNew()
        {
            return new FakeDisplayOrderRow();
        }

        /// <summary>ID field value</summary>
        public Int32? ID
        {
            get { return _id; }
            set { Fields.ID[this] = value; }
        }

        /// <summary>GroupID field value</summary>
        public Int32? GroupID
        {
            get { return _groupId; }
            set { Fields.GroupID[this] = value; }
        }

        /// <summary>DisplayOrder field value</summary>
        public Int32? DisplayOrder
        {
            get { return _displayOrder; }
            set { Fields.DisplayOrder[this] = value; }
        }

        /// <summary>IsActive field value</summary>
        public Int16? IsActive
        {
            get { return _isActive; }
            set { Fields.IsActive[this] = value; }
        }

        /// <summary>Implements IIdRow.IdField by returning the identity field</summary>
        IIdField IIdRow.IdField
        {
            get { return Fields.GroupID; }
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