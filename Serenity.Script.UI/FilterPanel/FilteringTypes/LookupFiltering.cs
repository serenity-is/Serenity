using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class LookupFiltering : BaseEditorFiltering
    {
        public LookupFiltering()
            : base(typeof(LookupEditor))
        {
        }

        public override List<FilterOperator> GetOperators()
        {
            return AppendNullableOperators(
                new List<FilterOperator>
                {
                    FilterOperators.EQ,
                    FilterOperators.NE,
                    FilterOperators.Contains,
                    FilterOperators.StartsWith
                });
        }

        protected override bool UseEditor()
        {
            return Operator.Key == FilterOperators.EQ || 
                Operator.Key == FilterOperators.NE;
        }

        protected override bool UseIdField()
        {
            return UseEditor();
        }

        protected override string GetEditorText()
        {
            if (UseEditor())
                return editor.As<LookupEditor>().Text;

            return base.GetEditorText();
        }
    }
}