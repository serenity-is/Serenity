using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class FilterStore
    {
        private EventHandler changed;
        private string displayText;

        public FilterStore(List<PropertyItem> fields)
        {
            Items = new List<FilterLine>();

            if (fields == null)
                throw new ArgumentNullException("source");

            this.Fields = fields.Clone();
            this.Fields.Sort((x, y) => Q.Externals.TurkishLocaleCompare(
                Q.TryGetText(x.Title) ?? x.Title ?? x.Name, 
                Q.TryGetText(y.Title) ?? y.Title ?? y.Name));
        
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

            if (changed != null)
                changed(this, EventArgs.Empty);
        }

        public event EventHandler Changed
        {
            add { changed += value; }
            remove { changed -= value; }
        }

        public BaseCriteria ActiveCriteria
        {
            get
            {
                return GetCriteriaFor(Items);
            }
        }

        public static BaseCriteria GetCriteriaFor(List<FilterLine> items)
        {
            if (items == null)
                return Criteria.Empty;

            bool inParens = false;
            BaseCriteria currentBlock = Criteria.Empty;
            bool isBlockOr = false;
            BaseCriteria criteria = Criteria.Empty;

            for (int i = 0; i < items.Count; i++)
            {
                var line = items[i];

                if (line.LeftParen || (inParens && line.RightParen))
                {
                    if (!currentBlock.IsEmpty)
                    {
                        if (inParens)
                            currentBlock = ~(currentBlock);

                        if (isBlockOr)
                            criteria |= currentBlock;
                        else
                            criteria &= currentBlock;

                        currentBlock = Criteria.Empty;
                    }

                    inParens = false;
                }

                if (line.LeftParen)
                {
                    isBlockOr = line.IsOr;
                    inParens = true;
                }

                if (line.IsOr)
                    currentBlock |= line.Criteria;
                else
                    currentBlock &= line.Criteria;
            }

            if (!currentBlock.IsEmpty)
            {
                if (isBlockOr)
                    criteria |= ~(currentBlock);
                else
                    criteria &= ~(currentBlock);
            }

            return criteria;
        }

        public static string GetDisplayTextFor(List<FilterLine> items)
        {
            bool inParens = false;
            var displayText = "";

            if (items == null)
                return displayText;

            for (int i = 0; i < items.Count; i++)
            {
                var line = items[i];

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

            if (inParens)
                displayText += ")";

            return displayText;
        }

        public string DisplayText
        {
            get
            {
                if (displayText == null)
                    displayText = GetDisplayTextFor(Items);

                return displayText;
            }
        }
    }
}