using System;
using System.Collections.Generic;

namespace Serenity
{
    public class FilterStore
    {
        private EventHandler changed;
        private string displayText;
        private IFilterableSource source;

        public FilterStore(IFilterableSource source)
        {
            Items = new List<FilterLine>();

            if (source == null)
                throw new ArgumentNullException("source");

            this.source = source;
        }

        public IFilterableSource Source
        {
            get { return source; }
        }

        public List<FilterLine> Items { get; private set; }

        public void RaiseChanged()
        {
            displayText = null;

            if (changed != null)
                changed(this, EventArgs.Empty);
        }

        public event EventHandler Changed
        {
            add { changed += value; }
            remove { changed -= value; }
        }

        public string DisplayText
        {
            get
            {
                if (displayText == null)
                {
                    bool inParens = false;
                    displayText = "";

                    for (int i = 0; i < Items.Count; i++)
                    {
                        var line = Items[i];

                        if (inParens && (line.RightParen || line.LeftParen))
                        {
                            displayText += ")";
                            inParens = false;
                        }

                        if (displayText.Length > 0)
                            displayText += " " + Q.Text("Controls.FilterPanel." + (line.IsOr ? "Or" : "And")) + " ";

                        if (line.LeftParen)
                        {
                            displayText += "(";
                            inParens = true;
                        }

                        displayText += line.DisplayText;
                    }
                }

                return displayText;
            }
        }
    }
}