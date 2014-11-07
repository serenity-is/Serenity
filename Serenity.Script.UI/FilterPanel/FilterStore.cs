using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity
{
    public class FilterStore
    {
        private EventHandler changed;
        private string displayText;

        public FilterStore(IEnumerable<PropertyItem> fields)
        {
            Items = new List<FilterLine>();

            if (fields == null)
                throw new ArgumentNullException("source");

            this.Fields = fields.ToList();
            this.Fields.Sort((x, y) => ((x.Title ?? x.Name).CompareTo(y.Title ?? y.Name)));
        
            this.FieldByName = new JsDictionary<string,PropertyItem>();

            foreach (var field in fields)
                FieldByName[field.Name] = field;
        }

        public List<PropertyItem> Fields { get; private set; }
        public JsDictionary<string, PropertyItem> FieldByName { get; private set; }
        public List<FilterLine> Items { get; private set; }

        public void RaiseChanged()
        {
            displayText = null;

            Q.Log(this.Items);

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