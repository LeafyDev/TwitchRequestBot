// ---------------------------------------------------------
// Copyrights (c) 2014-2018 LeafyDev 🍂 All rights reserved.
// ---------------------------------------------------------

using System;
using System.Windows.Forms;

namespace RequestBotThing
{
    internal sealed partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        // Save
        private void Button1_Click(object sender, EventArgs e)
        {
            userSettings.Default.requestConnected = textBox1.Text;
            userSettings.Default.requestCommand = textBox2.Text;
            userSettings.Default.requestAdded = textBox3.Text;
            userSettings.Default.requestChanged = textBox4.Text;
            userSettings.Default.requestPause = textBox5.Text;
            userSettings.Default.requestUnpause = textBox6.Text;
            userSettings.Default.requestDisabled = textBox7.Text;
            userSettings.Default.requestRemoved = textBox8.Text;
            userSettings.Default.removeCommand = textBox9.Text;
            userSettings.Default.newFollow = textBox10.Text;
            userSettings.Default.newSub = textBox11.Text;
            userSettings.Default.newResub = textBox12.Text;
            userSettings.Default.newDonation = textBox13.Text;
            userSettings.Default.newHost = textBox14.Text;
            userSettings.Default.newRaid = textBox15.Text;
            userSettings.Default.Save();
        }

        // Reset
        private void Button2_Click(object sender, EventArgs e)
        {
            userSettings.Default.requestAdded = "[user] has requested a song. (#[requestNumber] in line)";
            userSettings.Default.requestChanged = "[user] has changed their request. (#[requestNumber] in line)";
            userSettings.Default.requestCommand = "!request";
            userSettings.Default.requestConnected = "Requests are live! PogChamp";
            userSettings.Default.requestDisabled = "[user]: [channel] is not taking requests. BibleThump";
            userSettings.Default.requestPause = "[channel] is no longer taking requests. BibleThump";
            userSettings.Default.requestUnpause = "[channel] is now taking requests! PogChamp";
            userSettings.Default.requestRemoved = "Request by [user] was removed by a mod.";
            userSettings.Default.removeCommand = "!remove";
            userSettings.Default.newFollow = "Thanks [user] for following! PogChamp";
            userSettings.Default.newSub = "Thanks [user] for the sub! PogChamp";
            userSettings.Default.newResub = "[user] has resubbed for [amount] months! PogChamp";
            userSettings.Default.newDonation = "[user] is now hosting with [amount] viewers! PogChamp";
            userSettings.Default.newHost = "[user] is raiding with [amount] raiders! PogChamp";
            userSettings.Default.newRaid = "[user] just donated [amount]! PogChamp";
            textBox1.Text = userSettings.Default.requestConnected;
            textBox2.Text = userSettings.Default.requestCommand;
            textBox3.Text = userSettings.Default.requestAdded;
            textBox4.Text = userSettings.Default.requestChanged;
            textBox5.Text = userSettings.Default.requestPause;
            textBox6.Text = userSettings.Default.requestUnpause;
            textBox7.Text = userSettings.Default.requestDisabled;
            textBox8.Text = userSettings.Default.requestRemoved;
            textBox9.Text = userSettings.Default.removeCommand;
            textBox10.Text = userSettings.Default.newFollow;
            textBox11.Text = userSettings.Default.newSub;
            textBox12.Text = userSettings.Default.newResub;
            textBox13.Text = userSettings.Default.newDonation;
            textBox14.Text = userSettings.Default.newHost;
            textBox15.Text = userSettings.Default.newRaid;

            MessageBox.Show(@"Messages were reset, but not saved, click save to apply changes.");
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = userSettings.Default.requestConnected;
            textBox2.Text = userSettings.Default.requestCommand;
            textBox3.Text = userSettings.Default.requestAdded;
            textBox4.Text = userSettings.Default.requestChanged;
            textBox5.Text = userSettings.Default.requestPause;
            textBox6.Text = userSettings.Default.requestUnpause;
            textBox7.Text = userSettings.Default.requestDisabled;
            textBox8.Text = userSettings.Default.requestRemoved;
            textBox9.Text = userSettings.Default.removeCommand;
            textBox10.Text = userSettings.Default.newFollow;
            textBox11.Text = userSettings.Default.newSub;
            textBox12.Text = userSettings.Default.newResub;
            textBox13.Text = userSettings.Default.newDonation;
            textBox14.Text = userSettings.Default.newHost;
            textBox15.Text = userSettings.Default.newRaid;
        }
    }
}