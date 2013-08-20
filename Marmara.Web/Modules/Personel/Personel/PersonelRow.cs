namespace Marmara.Personel.Entities
{
    using Serenity;
    using Serenity.Data;
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel;
    using R = PersonelRow;

    /// <summary>
    ///   Entity class for "Personel" table</summary>
    [Schema("Personel"), DisplayName("Personel"), InstanceName("Personel")]
    [ReadPermission("PersonelYonetimi")]
    [ModifyPermission("PersonelYonetimi")]
    [JsonConverter(typeof(JsonRowConverter))]
    public sealed class PersonelRow : LoggingRow, IIdRow, INameRow
    {
        /// <summary>Table name</summary>
        public const string TableName = "Personel";

        public class RowFields : LoggingRowFields
        {
            /// <summary>PersonelID field</summary>
            public readonly Int32Field PersonelID;
            /// <summary>KimlikNo field</summary>
            public readonly StringField KimlikNo;
            /// <summary>Adi field</summary>
            public readonly StringField Adi;
            /// <summary>Soyadi field</summary>
            public readonly StringField Soyadi;
            /// <summary>KizlikSoyadi field</summary>
            public readonly StringField KizlikSoyadi;
            /// <summary>PersonelTip field</summary>
            public readonly Int32Field PersonelTip;
            /// <summary>MemuriyeteIlkBaslamaSoyad field</summary>
            public readonly StringField MemuriyeteIlkBaslamaSoyad;

            /// <summary>PersonelTip join</summary>
            public readonly LeftJoin PersonelTipJoin;
            /// <summary>PersonelTipPersonelTipID field</summary>
            public readonly Int32Field PersonelTipPersonelTipID;
            /// <summary>PersonelTipKodGrup field</summary>
            public readonly Int32Field PersonelTipKodGrup;
            /// <summary>PersonelTipKodNo field</summary>
            public readonly Int32Field PersonelTipKodNo;
            /// <summary>PersonelTipAciklama field</summary>
            public readonly StringField PersonelTipAciklama;
            /// <summary>PersonelTipAciklamaKisa field</summary>
            public readonly StringField PersonelTipAciklamaKisa;
            /// <summary>PersonelTipGizli field</summary>
            public readonly BooleanField PersonelTipGizli;
            /// <summary>PersonelTipGuncellenebilir field</summary>
            public readonly BooleanField PersonelTipGuncellenebilir;

            public RowFields()
                : base(R.TableName, "")
            {
                LocalTextPrefix = "Personel.Personel";

                PersonelID = new Int32Field(this, "PersonelID", "PersonelID", 10, FieldFlags.Identity,
                    r => ((R)r)._PersonelID, (r, v) => ((R)r)._PersonelID = v);

                KimlikNo = new StringField(this, "KimlikNo", "KimlikNo", 15, FieldFlags.Required,
                    r => ((R)r)._KimlikNo, (r, v) => ((R)r)._KimlikNo = v)
                {
                    MinSelectLevel = SelectLevel.Lookup
                };

                Adi = new StringField(this, "Adi", "Adi", 30, FieldFlags.Required,
                    r => ((R)r)._Adi, (r, v) => ((R)r)._Adi = v);

                Soyadi = new StringField(this, "Soyadi", "Soyadi", 30, FieldFlags.Required,
                    r => ((R)r)._Soyadi, (r, v) => ((R)r)._Soyadi = v);

                KizlikSoyadi = new StringField(this, "KizlikSoyadi", "KizlikSoyadi", 30, FieldFlags.Required,
                    r => ((R)r)._KizlikSoyadi, (r, v) => ((R)r)._KizlikSoyadi = v);

                PersonelTip = new Int32Field(this, "PersonelTip", "PersonelTip", 10, FieldFlags.Required,
                    r => ((R)r)._PersonelTip, (r, v) => ((R)r)._PersonelTip = v)
                {
                    ForeignTable = "Kod",
                    ForeignField = "KodID"
                };

                MemuriyeteIlkBaslamaSoyad = new StringField(this, "MemuriyeteIlkBaslamaSoyad", "MemuriyeteIlkBaslamaSoyad", 30, FieldFlags.Default,
                    r => ((R)r)._MemuriyeteIlkBaslamaSoyad, (r, v) => ((R)r)._MemuriyeteIlkBaslamaSoyad = v);

                PersonelTipJoin = PersonelTip.ForeignJoin();
                PersonelTipPersonelTipID = new Int32Field(this, "PersonelTipPersonelTipID", PersonelTip).SetOrigin(PersonelTipJoin, "PersonelTipID");
                PersonelTipKodGrup = new Int32Field(this, "PersonelTipKodGrup", PersonelTip).SetOrigin(PersonelTipJoin, "KodGrup");
                PersonelTipKodNo = new Int32Field(this, "PersonelTipKodNo", PersonelTip).SetOrigin(PersonelTipJoin, "KodNo");
                PersonelTipAciklama = new StringField(this, "PersonelTipAciklama", PersonelTip).SetOrigin(PersonelTipJoin, "Aciklama");
                PersonelTipAciklamaKisa = new StringField(this, "PersonelTipAciklamaKisa", PersonelTip).SetOrigin(PersonelTipJoin, "AciklamaKisa");
                PersonelTipGizli = new BooleanField(this, "PersonelTipGizli", PersonelTip).SetOrigin(PersonelTipJoin, "Gizli");
                PersonelTipGuncellenebilir = new BooleanField(this, "PersonelTipGuncellenebilir", PersonelTip).SetOrigin(PersonelTipJoin, "Guncellenebilir");

            }
        }

        /// <summary>Row fields</summary>
        public static readonly RowFields Fields;
        /// <summary>Shared instance of the row class</summary>
        public static readonly PersonelRow Instance;

        /// <summary>Initializes field definitions</summary>
        static PersonelRow()
        {
            Fields = new RowFields();
            Instance = new PersonelRow();
        }

        private Int32? _PersonelID;
        private String _KimlikNo;
        private String _Adi;
        private String _Soyadi;
        private String _KizlikSoyadi;
        private Int32? _PersonelTip;
        private String _MemuriyeteIlkBaslamaSoyad;

        /// <summary>Creates a new PersonelRow instance.</summary>
        public PersonelRow()
            : base(Fields)
        {
        }

        /// <summary>Creates a new PersonelRow instance.</summary>
        public override Row CreateNew()
        {
            return new PersonelRow();
        }
        
        /// <summary>PersonelID field value</summary>
        public Int32? PersonelID
        {
            get { return _PersonelID; }
            set { Fields.PersonelID[this] = value; }
        }

        /// <summary>KimlikNo field value</summary>
        public String KimlikNo
        {
            get { return _KimlikNo; }
            set { Fields.KimlikNo[this] = value; }
        }

        /// <summary>Adi field value</summary>
        public String Adi
        {
            get { return _Adi; }
            set { Fields.Adi[this] = value; }
        }

        /// <summary>Soyadi field value</summary>
        public String Soyadi
        {
            get { return _Soyadi; }
            set { Fields.Soyadi[this] = value; }
        }

        /// <summary>KizlikSoyadi field value</summary>
        public String KizlikSoyadi
        {
            get { return _KizlikSoyadi; }
            set { Fields.KizlikSoyadi[this] = value; }
        }

        /// <summary>PersonelTip field value</summary>
        public Int32? PersonelTip
        {
            get { return _PersonelTip; }
            set { Fields.PersonelTip[this] = value; }
        }

        /// <summary>MemuriyeteIlkBaslamaSoyad field value</summary>
        public String MemuriyeteIlkBaslamaSoyad
        {
            get { return _MemuriyeteIlkBaslamaSoyad; }
            set { Fields.MemuriyeteIlkBaslamaSoyad[this] = value; }
        }

        /// <summary>PersonelTipPersonelTipID field value</summary>
        public Int32? PersonelTipPersonelTipID
        {
            get { return Fields.PersonelTipPersonelTipID[this]; }
            set { Fields.PersonelTipPersonelTipID[this] = value; }
        }

        /// <summary>PersonelTipKodGrup field value</summary>
        public Int32? PersonelTipKodGrup
        {
            get { return Fields.PersonelTipKodGrup[this]; }
            set { Fields.PersonelTipKodGrup[this] = value; }
        }

        /// <summary>PersonelTipKodNo field value</summary>
        public Int32? PersonelTipKodNo
        {
            get { return Fields.PersonelTipKodNo[this]; }
            set { Fields.PersonelTipKodNo[this] = value; }
        }

        /// <summary>PersonelTipAciklama field value</summary>
        public String PersonelTipAciklama
        {
            get { return Fields.PersonelTipAciklama[this]; }
            set { Fields.PersonelTipAciklama[this] = value; }
        }

        /// <summary>PersonelTipAciklamaKisa field value</summary>
        public String PersonelTipAciklamaKisa
        {
            get { return Fields.PersonelTipAciklamaKisa[this]; }
            set { Fields.PersonelTipAciklamaKisa[this] = value; }
        }

        /// <summary>PersonelTipGizli field value</summary>
        public Boolean? PersonelTipGizli
        {
            get { return Fields.PersonelTipGizli[this]; }
            set { Fields.PersonelTipGizli[this] = value; }
        }

        /// <summary>PersonelTipGuncellenebilir field value</summary>
        public Boolean? PersonelTipGuncellenebilir
        {
            get { return Fields.PersonelTipGuncellenebilir[this]; }
            set { Fields.PersonelTipGuncellenebilir[this] = value; }
        }

        /// <summary>Implements IIdRow.IdField by returning the identity field</summary>
        IIdField IIdRow.IdField
        {
            get { return Fields.PersonelID; }
        }

        /// <summary>Implements INameRow.NameField by returning KimlikNo field</summary>
        StringField INameRow.NameField
        {
            get { return Fields.KimlikNo; }
        }
    }
}