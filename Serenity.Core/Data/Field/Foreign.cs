using System;

namespace Serenity.Data
{
    /// <summary>
    ///   Bir kaydýn join'lerden gelen alanlarý için kullanýlabilecek, baðlý olduðu join'i ve kaynak
    ///   expression'ýný içinde saklayan Field alan sýnýfý</summary>
    public struct Foreign
    {
        /// <summary>
        ///   kaynak join, "T0" gibi, birden fazla join alanýnýn karýþýmýndan elde edilen,
        ///   ör. "T0.X + T1.Y" gibi alanlar için null olabilir</summary>
        private string _joinAlias;
        /// <summary>
        ///   kaynak ifade, source join dýþýndaki ifadeyi içerir. Ör. alanýn ifadesi "T0.KOD" ise
        ///   expression "KOD" olmalý. Alan adýyla ifade ayný olduðunda bu saha null olabilir.</summary>
        private string _joinField;

        ///// <summary>
        /////   Verilen kaynak join, kaynak alan ve alias'a sahip yeni bir Foreign nesnesi 
        /////   oluþturur.</summary>
        ///// <param name="sourceJoin">
        /////   Kaynak join. "T0" gibi. Null olamaz.</param>
        ///// <param name="sourceField">
        /////   Kaynak alan. "T0.KOD" için "KOD" geçirilmeli. Null olamaz.</param>
        ///// <param name="name">
        /////   Alana select sorgularýnda atanacak alias. Null olamaz.</param>
        //public Foreign(string joinAlias, string joinField)
        //{
        //    if (joinAlias == null)
        //        throw new ArgumentNullException("joinAlias");
        //    if (joinField == null)
        //        throw new ArgumentNullException("joinField");
        //    _joinAlias = joinAlias;
        //    _joinField = joinField;
        //}

        public Foreign(LeftJoin join, string joinField)
        {
            if (join == null)
                throw new ArgumentNullException("join");
            if (joinField == null)
                throw new ArgumentNullException("joinField");
            _joinAlias = join.JoinAlias;
            _joinField = joinField;
        }

        /// <summary>
        ///   Alanýn oluþturulmasý sýrasýnda atanan kaynak join'i verir ("T0" gibi).
        ///   Birden fazla join tablosundan hesaplanan karmaþýk alanlar için null olabilir.</summary>
        public string JoinAlias
        {
            get { return _joinAlias; }
        }

        /// <summary>
        ///   Alanýn oluþturulmasý sýrasýnda atanan ifadeyi verir. ("KOD" gibi).
        ///   Bu ifade null ise alan adý ile ifade ayný demektir. Ýfade kullanýlýrken
        ///   SourceJoin ile birlikte deðerlendirilmelidir.</summary>
        public string JoinField
        {
            get { return _joinField; }
        }
    }
}