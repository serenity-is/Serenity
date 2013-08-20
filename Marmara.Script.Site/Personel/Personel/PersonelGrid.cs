
namespace Marmara.Personel
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("PersonelID"), NameProperty("KimlikNo"), IsActiveProperty("Aktif")]
    public class PersonelGrid : EntityGrid<PersonelRow, GridOptions>
    {
        public PersonelGrid(jQueryObject container, GridOptions options)
            : base(container, options)
        {
        }

        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "PersonelID", Width = 55, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "KimlikNo", Width = 200, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "Adi", Width = 80 });
            columns.Add(new SlickColumn { Field = "Soyadi", Width = 80 });
            columns.Add(new SlickColumn { Field = "KizlikSoyadi", Width = 80 });
            columns.Add(new SlickColumn { Field = "PersonelTip", Width = 80 });
            columns.Add(new SlickColumn { Field = "MemuriyeteIlkBaslamaSoyad", Width = 80 });

            return columns;
        }
    }
}