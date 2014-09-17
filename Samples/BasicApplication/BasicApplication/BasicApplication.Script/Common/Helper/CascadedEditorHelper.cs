namespace BasicApplication.Common
{
    using Serenity;
    using System;

    public class CascadedEditorHelper<TWidget, TParentWidget>
        where TWidget: Widget
        where TParentWidget: Widget
    {
        private TWidget widget;
        private Action updateItems;
        private string parentID;
        private object parentValue;
        private Func<TParentWidget, object> getParentValue;

        public CascadedEditorHelper(TWidget widget, Func<TParentWidget, object> getParentValue, Action updateItems)
        {
            this.widget = widget;
            this.updateItems = updateItems;
            this.getParentValue = getParentValue;
        }

        public void BindToParent()
        {
            if (ParentID.IsEmptyOrNull())
                return;

            var parent = Q.FindElementWithRelativeId(widget.Element, ParentID).TryGetWidget<TParentWidget>();
            if (parent != null)
            {
                parent.Element.Bind("change." + widget.UniqueName, delegate
                {
                    ParentValue = getParentValue(parent);
                });
            }
        }

        public void UnbindFromParent()
        {
            if (ParentID.IsEmptyOrNull())
                return;

            var parent = Q.FindElementWithRelativeId(widget.Element, ParentID).TryGetWidget<TParentWidget>();
            if (parent != null)
                parent.Element.Unbind("." + widget.UniqueName);
        }

        public string ParentID
        {
            get
            {
                return parentID;
            }
            set
            {
                if (parentID != value)
                {
                    UnbindFromParent();
                    parentID = value;
                    BindToParent();
                    updateItems();
                }
            }
        }

        public object ParentValue
        {
            get { return parentValue; }
            set
            {
                if ((parentValue ?? "").ToString() != (value ?? "").ToString())
                {
                    this.parentValue = value;
                    updateItems();
                }
            }
        }
    }
}