using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Samples
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(SqlQuerySamples.Sample1_Listing1_SimpleSelectFromOrderBy());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(SqlQuerySamples.Sample1_Listing2_SimpleSelectReordered());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(SqlQuerySamples.Sample1_Listing3_SimpleSelectMethodChaining());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(SqlQuerySamples.Sample1_Listing4_SimpleSelectRemoveVariable());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show(SqlQuerySamples.Sample1_Listing5_CrossJoin());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show(SqlQuerySamples.Sample1_Listing6_FromAs());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MessageBox.Show(SqlQuerySamples.Sample1_Listing7_FromAsAlias());
        }
    }
}