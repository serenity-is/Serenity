
namespace Marmara.Personel
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("PersonelID"), NameProperty("KimlikNo"), IsActiveProperty("Aktif")]
    public class PersonelDialog : EntityDialog<PersonelRow, object>
    {
        public PersonelDialog(jQueryObject container)
            : base(new object())
        {
        }
    }
}