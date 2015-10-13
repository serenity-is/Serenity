namespace Serenity
{
    using System;

    public class CascadedWidgetLink<TParent>
        where TParent: Widget
    {
        private Widget widget;
        private Action<TParent> parentChange;

        public CascadedWidgetLink(Widget widget, Action<TParent> parentChange)
        {
            this.widget = widget;
            this.parentChange = parentChange;
            Bind();
            this.widget.Element.Bind("remove." + widget.UniqueName + "cwh", (e) => {
                Unbind();
                this.widget = null;
                this.parentChange = null;
            });
        }

        public void Bind()
        {
            if (parentID.IsEmptyOrNull())
                return;

            var parent = Q.FindElementWithRelativeId(widget.Element, parentID).TryGetWidget<TParent>();
            if (parent != null)
            {
                parent.Element.Bind("change." + widget.UniqueName, delegate
                {
                    parentChange(parent);
                });
            }
        }

        public void Unbind()
        {
            if (parentID.IsEmptyOrNull())
                return;

            var parent = Q.FindElementWithRelativeId(widget.Element, parentID).TryGetWidget<TParent>();
            if (parent != null)
                parent.Element.Unbind("." + widget.UniqueName);
        }

        private string parentID;

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
                    Unbind();
                    parentID = value;
                    Bind();
                }
            }
        }
    }
}