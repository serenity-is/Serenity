namespace Marmara.Personel
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported, Serializable, PreserveMemberCase]
    public sealed class PersonelRow
    {
        public Int32? PersonelID { get; set; }
        public String KimlikNo { get; set; }
        public String Adi { get; set; }
        public String Soyadi { get; set; }
        public String KizlikSoyadi { get; set; }
        public Int32? PersonelTip { get; set; }
        public String MemuriyeteIlkBaslamaSoyad { get; set; }
        public Int32? PersonelTipPersonelTipID { get; set; }
        public Int32? PersonelTipKodGrup { get; set; }
        public Int32? PersonelTipKodNo { get; set; }
        public String PersonelTipAciklama { get; set; }
        public String PersonelTipAciklamaKisa { get; set; }
        public Boolean? PersonelTipGizli { get; set; }
        public Boolean? PersonelTipGuncellenebilir { get; set; }
    }
}