using System.Windows.Forms;

namespace RootProjectWizard
{
    public partial class FeatureSelection : Form
    {
        public FeatureSelection()
        {
            InitializeComponent();
        }

        public int selfChange;

        private void Uncheck(bool[] checks, int index)
        {
            if (!checks[index])
                return;

            selfChange++;
            try
            {
                featureList.SetItemCheckState(index, CheckState.Unchecked);
            }
            finally
            {
                selfChange--;
            }

            checks[index] = false;
            var key = ((FeatureCheckItem)featureList.Items[index]).Key;
            for (var i = 0; i < checks.Length; i++)
                if (checks[i])
                {
                    var item = (FeatureCheckItem)featureList.Items[i];
                    if (item.FeatureDependencies.Contains(key))
                        Uncheck(checks, i);
                }
        }

        private void Check(bool[] checks, int index)
        {
            if (checks[index])
                return;

            selfChange++;
            try
            {
                featureList.SetItemCheckState(index, CheckState.Checked);
            }
            finally
            {
                selfChange--;
            }
            checks[index] = true;
            foreach (var key in ((FeatureCheckItem)featureList.Items[index]).FeatureDependencies)
            {
                for (var i = 0; i < checks.Length; i++)
                    if (!checks[i])
                    {
                        var item = (FeatureCheckItem)featureList.Items[i];
                        if (item.Key == key)
                            Check(checks, i);
                    }
            }
        }

        private void featureList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (selfChange > 0)
                return;

            if (e.CurrentValue == e.NewValue)
                return;

            var checks = new bool[featureList.Items.Count];
            foreach (int x in featureList.CheckedIndices)
                checks[x] = true;

            var item = featureList.Items[e.Index] as FeatureCheckItem;
            if (e.NewValue == CheckState.Unchecked)
            {
                Uncheck(checks, e.Index);
            }
            else
            {
                Check(checks, e.Index);
            }
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {

        }
    }
}
