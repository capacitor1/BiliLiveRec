using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BiliLiveRec
{
    public partial class Input : Form
    {
        public Input()
        {
            InitializeComponent();
        }
        public string Result = string.Empty;
        private void Ok_Click(object sender, EventArgs e)
        {
            Result = Content.Text;
            this.Close();
        }
        public string GetResult()
        {
            this.ShowDialog();
            return Result;
        }

        private void Content_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                Ok_Click(sender, e);
            }
        }
    }
}
