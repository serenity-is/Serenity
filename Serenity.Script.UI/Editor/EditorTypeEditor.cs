using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Editör Tipi")]
    [Element("<input type=\"hidden\" />")]
    public class EditorTypeEditor : SelectEditor, IStringValue
    {
        private static List<object> editorTypeList;

        public EditorTypeEditor(jQueryObject select)
            : base(select, new SelectEditorOptions { EmptyOptionText = "--seçiniz--" })
        {
        }

        protected override List<object> GetItems()
        {
            if (editorTypeList == null)
            {
                editorTypeList = new List<object>();
                foreach (var info in EditorTypeCache.RegisteredTypes)
                    editorTypeList.Add(new object[] { info.Key, info.Value.DisplayName });

                editorTypeList.Sort((x, y) =>
                {
                    var xn = x.As<object[]>()[1].As<string>();
                    var yn = y.As<object[]>()[1].As<string>();
                    return Q.Externals.TurkishLocaleCompare(xn, yn);
                });
            }

            return editorTypeList;
        }
    }
}