using System.Collections.Generic;

namespace RootProjectWizard
{
    public class FeatureCheckItem
    {
        public FeatureCheckItem(string key, string title)
        {
            this.Key = key;
            this.Title = title;
            this.FeatureDependencies = new List<string>();
        }

        public string Key { get; set; }
        public string Title { get; set; }
        public List<string> FeatureDependencies { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
