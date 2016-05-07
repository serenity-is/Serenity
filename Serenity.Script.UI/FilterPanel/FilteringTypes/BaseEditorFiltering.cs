using jQueryApi;
using Serenity.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public abstract class BaseEditorFiltering<TEditor> : BaseFiltering
        where TEditor: Widget
    {
        protected Widget editor;

        protected virtual bool UseEditor()
        {
            switch (Operator.Key)
            {
                case FilterOperators.EQ:
                case FilterOperators.NE:
                case FilterOperators.LT:
                case FilterOperators.LE:
                case FilterOperators.GT:
                case FilterOperators.GE:
                    return true;
            }

            return false;
        }

        public override void CreateEditor()
        {
            if (UseEditor())
            {
                editor = Widget.CreateInside<TEditor>(Container, GetEditorOptions());
                return;
            }

            base.CreateEditor();
        }

        protected virtual bool UseIdField()
        {
            return false;
        }

        protected override string GetCriteriaField()
        {
            if (UseEditor() &&
                UseIdField() &&
                !Field.FilteringIdField.IsEmptyOrNull())
            {
                return Field.FilteringIdField as string;
            }

            return base.GetCriteriaField();
        }

        protected virtual object GetEditorOptions()
        {
            var opt = Q.DeepExtend(new JsDictionary(), Field.EditorParams);
            Script.Delete(opt, "cascadeFrom"); // currently can't support cascadeFrom in filtering
            return Q.DeepExtend(opt, Field.FilteringParams);
        }

        public override void LoadState(object state)
        {
            if (UseEditor())
            {
                if (state == null)
                    return;

                EditorUtils.SetValue(editor, state);
                return;
            }

            base.LoadState(state);
        }

        public override object SaveState()
        {
            if (UseEditor())
                return EditorUtils.GetValue(editor);

            return base.SaveState();
        }

        protected override object GetEditorValue()
        {
            if (UseEditor())
            {
                var value = EditorUtils.GetValue(editor);
                if (value == null || (value is string && ((string)value).Trim().Length == 0))
                    throw ArgumentNull();

                return value;
            }

            return base.GetEditorValue();
        }

        public override void InitQuickFilter(QuickFilter<Widget, object> filter)
        {
            base.InitQuickFilter(filter);
            filter.Type = typeof(TEditor);
            filter.Options = Q.DeepExtend(new JsDictionary<string, object>(), GetEditorOptions(), Field.QuickFilterParams);
        }
    }
}