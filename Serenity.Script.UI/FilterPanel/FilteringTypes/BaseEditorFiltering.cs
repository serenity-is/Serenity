using jQueryApi;
using Serenity.Data;
using System;
using System.Collections;
using System.Collections.Generic;

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
            return Field.FilteringParams;
        }

        public override void LoadState(object state)
        {
            if (UseEditor())
            {
                if (state == null)
                    return;

                PropertyGrid.LoadEditorValue(editor, new PropertyItem { Name = "_" }, state);
                return;
            }

            base.LoadState(state);
        }

        public override object SaveState()
        {
            if (UseEditor())
            {
                var target = new JsDictionary();
                PropertyGrid.SaveEditorValue(editor, new PropertyItem { Name = "_" }, target);
                return target;
            }

            return base.SaveState();
        }

        protected override object GetEditorValue()
        {
            if (UseEditor())
            {
                var target = new JsDictionary();
                PropertyGrid.SaveEditorValue(editor, new PropertyItem { Name = "_" }, target);
                var value = target["_"];
                if (value == null || (value is string && ((string)value).Trim().Length == 0))
                    throw ArgumentNull();

                return target["_"];
            }

            return base.GetEditorValue();
        }
    }
}