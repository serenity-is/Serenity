using System.Collections.Generic;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class EditorFiltering : BaseEditorFiltering
    {
        public EditorFiltering()
            : base(typeof(Widget))
        {
        }

        [Option, IntrinsicProperty]
        public string EditorType { get; set; }
        [Option, IntrinsicProperty]
        public bool UseRelative { get; set; }
        [Option, IntrinsicProperty]
        public bool UseLike { get; set; }

        public override List<FilterOperator> GetOperators()
        {
            var list = new List<FilterOperator>();
            list.Add(FilterOperators.EQ);
            list.Add(FilterOperators.NE);

            if (UseRelative)
            {
                list.Add(FilterOperators.LT);
                list.Add(FilterOperators.LE);
                list.Add(FilterOperators.GT);
                list.Add(FilterOperators.GE);
            }

            if (UseLike)
            {
                list.Add(FilterOperators.Contains);
                list.Add(FilterOperators.StartsWith);
            }

            AppendNullableOperators(list);

            return list;
        }

        protected override bool UseEditor()
        {
            return Operator.Key == FilterOperators.EQ ||
                Operator.Key == FilterOperators.NE ||
                (UseRelative &&
                 (Operator.Key == FilterOperators.LT ||
                  Operator.Key == FilterOperators.LE ||
                  Operator.Key == FilterOperators.GT ||
                  Operator.Key == FilterOperators.GE));
        }

        protected override object GetEditorOptions()
        {
            var opt = base.GetEditorOptions();

            if (UseEditor() && EditorType == (Field.EditorType ?? "String"))
                opt = jQuery.Extend(opt, Field.EditorParams);

            return opt;
        }

        public override void CreateEditor()
        {
            if (UseEditor())
            {
                var editorType = EditorTypeRegistry.Get(EditorType);
                editor = Widget.CreateOfType(editorType, e => e.AppendTo(Container), GetEditorOptions());
                return;
            }

            base.CreateEditor();
        }

        protected override bool UseIdField()
        {
            return UseEditor();
        }

        protected override string GetEditorText()
        {
            return base.GetEditorText();
        }

        public override void InitQuickFilter(QuickFilter<Widget, object> filter)
        {
            base.InitQuickFilter(filter);

            filter.Type = EditorTypeRegistry.Get(EditorType);
        }
    }
}