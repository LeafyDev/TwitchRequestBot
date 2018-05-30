// ---------------------------------------------------------
// Copyrights (c) 2014-2018 LeafyDev 🍂 All rights reserved.
// ---------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RequestBotThing
{
    internal sealed partial class slForm : Form
    {
        public slForm()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Settings.Default.slKey = textBox1.Text;
            Settings.Default.Save();
            Close();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(@"Click on 'API Tokens' then copy the 'API Access Token' in the box");
            Process.Start("https://streamlabs.com/dashboard#/apisettings");
        }
    }
}