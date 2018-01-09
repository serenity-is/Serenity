namespace Serenity
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported(ObeysTypeSystem = true), IncludeGenericArguments(false)]
    public class CascadedWidgetLink<TParent>
        where TParent: Widget
    {
        private Widget widget;
        private Type parentType;
        private Action<TParent> parentChange;

        public CascadedWidgetLink(Type parentType, Widget widget, Action<TParent> parentChange)
        {
            this.parentType = parentType;
            this.widget = widget;
            this.parentChange = parentChange;
            Bind();
            this.widget.Element.Bind("remove." + widget.UniqueName + "cwh", (e) => {
                Unbind();
                this.widget = null;
                this.parentChange = null;
            });
        }

        public TParent Bind()
        {
            if (parentID.IsEmptyOrNull())
                return null;

            var parent = Q.FindElementWithRelativeId(widget.Element, parentID).TryGetWidget(parentType).As<TParent>();
            if (parent != null)
            {
                parent.Element.Bind("change." + widget.UniqueName, delegate
                {
                    parentChange(parent);
                });

                return parent;
            }
            else
            {
                Q.NotifyError("Can't find cascaded parent element with ID: " + parentID + "!");
                return null;
            }
        }

        public TParent Unbind()
        {
            if (parentID.IsEmptyOrNull())
                return null;

            var parent = Q.FindElementWithRelativeId(widget.Element, parentID).TryGetWidget(parentType).As<TParent>();
            if (parent != null)
                parent.Element.Unbind("." + widget.UniqueName);

            return parent;
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