using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncLearning
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public Form1 (FormActions actions) : this()
        {
            this.actions = actions;
            actions.AssignForm(this);
        }

        public void AddLog(string logText)
        {
            textBoxLog.Text += logText + Environment.NewLine;
        }

        private readonly FormActions actions;

        private void button1_Click(object sender, EventArgs e)
        {
            actions.OnClick();
        }
    }
}
