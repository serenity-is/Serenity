
namespace Marmara.Personel.Forms
{
    using System;
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System.Collections.Generic;

    [FormScript("Personel.Personel")]
    [BasedOnRow(typeof(Entities.PersonelRow))]
    public class PersonelForm
    {
        public String KimlikNo { get; set; }
        public String Adi { get; set; }
        public String Soyadi { get; set; }
        public String KizlikSoyadi { get; set; }
        public Int32 PersonelTip { get; set; }
        public String MemuriyeteIlkBaslamaSoyad { get; set; }
    }
}