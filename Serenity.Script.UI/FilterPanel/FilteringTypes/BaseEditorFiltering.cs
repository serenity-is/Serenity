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
        private Widget editor;

        protected bool IsComparison(string op)
        {
            switch (op)
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
            if (IsComparison(Operator.Key))
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

        protected override string GetFieldName()
        {
            if (UseIdField() &&
                Field.FilteringParams != null &&
                Field.FilteringParams.ContainsKey("idField"))
            {
                return Field.FilteringParams["idField"] as string;
            }

            return base.GetFieldName();
        }

        protected virtual object GetEditorOptions()
        {
            return Field.FilteringParams;
        }

        public override void LoadState(object state)
        {
            if (IsComparison(Operator.Key))
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
            if (IsComparison(Operator.Key))
            {
                var target = new JsDictionary();
                PropertyGrid.SaveEditorValue(editor, new PropertyItem { Name = "_" }, target);
                return target;
            }

            return base.SaveState();
        }

        protected override object GetEditorValue()
        {
            if (IsComparison(Operator.Key))
            {
                var target = new JsDictionary();
                PropertyGrid.SaveEditorValue(editor, new PropertyItem { Name = "_" }, target);
                return target["_"];
            }

            return base.SaveState();
        }

        protected override string GetEditorText()
        {
            if (IsComparison(Operator.Key))
                return Container.GetText();

            return GetEditorValue().ToString();
        }
    }
}