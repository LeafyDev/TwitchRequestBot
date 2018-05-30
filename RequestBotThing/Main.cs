// ---------------------------------------------------------
// Copyrights (c) 2014-2018 LeafyDev 🍂 All rights reserved.
// ---------------------------------------------------------

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

using StreamLabsDotNet.Client;
using StreamLabsDotNet.Client.Models;

using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace RequestBotThing
{
    internal partial class Main : Form
    {
        private const string version = "1.8.1";
        private static bool userDisconnect;
        private bool alertsOn;
        private JoinedChannel mainChannel;
        private Client slClient;
        private bool takingRequests = true;
        private int tmpRow;
        private TwitchClient twitchClient;

        public Main()
        {
            InitializeComponent();

            Application.ThreadException += ErrorHandler.Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += ErrorHandler.CurrentDomain_UnhandledException;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Text = $@"Request bot thing - LeafyDev (v{version})";

            if (File.Exists("latest.log")) File.Move("latest.log", $"{DateTime.Now:yy-MMM-dd_hh.mm.ss}.log");
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (Settings.Default.localVersion != version)
            {
                var changelog = $"What's new: {version}"
                                + "- Logger should be thread safe, if the bot crashes with any message other than , please send latest.log BEFORE restarting the bot."
                                + "- Hydration message now has a ~25% chance of showing, to be more of a secret wink."
                                + "- Allowed users to add a request while their current request is being played."
                                + "- Fixed what's new message actually showing for first start of new versions";

                MessageBox.Show(changelog.Replace(".-", $".{Environment.NewLine}-"));
                Settings.Default.localVersion = version;
                Settings.Default.Save();
            }

            if (!string.IsNullOrEmpty(Settings.Default.Username)) textBox1.Text = Settings.Default.Username;
            if (!string.IsNullOrEmpty(Settings.Default.Token)) textBox2.Text = Settings.Default.Token;
            if (!string.IsNullOrEmpty(Settings.Default.ChannelName)) textBox3.Text = Settings.Default.ChannelName;

            LabelChange(@"Status: Checking for update...");

            var webClient = new WebClient();
            var stream = webClient.OpenRead("http://g.whaskell.pw/requestbot/latest.txt");
            if (stream != null)
            {
                var reader = new StreamReader(stream);
                var webVersion = reader.ReadToEnd().Replace("\n", "");

                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (webVersion == version)
                {
                    LabelChange(@"Status: No updates available.");
                }
                else
                {
                    LabelChange(@"Status: Update is available!");
                    button5.Visible = true;
                }
            }
            else
            {
                LabelChange(@"Status: Failed to check for updates...");
            }
        }

        // Connect
        private void Button1_Click(object sender, EventArgs e)
        {
            switch (button1.Text)
            {
                case "Connect":
                    if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) ||
                        string.IsNullOrEmpty(textBox3.Text))
                    {
                        MessageBox.Show(
                            @"Please enter your username, channel name and token from https://twitchapps.com/tmi/ in the textboxes.");
                    }
                    else
                    {
                        userDisconnect = false;

                        twitchClient = new TwitchClient();

                        twitchClient.Initialize(new ConnectionCredentials(textBox1.Text, textBox2.Text), textBox3.Text);

                        twitchClient.OnJoinedChannel += OnJoinedChannel;
                        twitchClient.OnMessageReceived += OnMessageReceived;
                        twitchClient.OnDisconnected += OnDisconnected;
                        twitchClient.OnLog += OnLog;

                        twitchClient.Connect();

                        button1.Text = @"Disconnect";

                        Settings.Default.Username = textBox1.Text;
                        Settings.Default.Token = textBox2.Text;
                        Settings.Default.ChannelName = textBox3.Text;

                        Settings.Default.Save();
                    }

                    break;
                case "Disconnect":
                    userDisconnect = true;
                    twitchClient.Disconnect();
                    button1.Text = @"Connect";
                    break;
            }
        }

        private static void OnLog(object sender, OnLogArgs e) => Logger.WriteToFileThreadSafe($"[{e.DateTime:s}] {e.Data}");

        // Remove
        private void Button2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectedItem in listView1.SelectedItems)
                DelItem(selectedItem);
        }

        // Get Token
        private void Button3_Click(object sender, EventArgs e) => Process.Start("https://twitchapps.com/tmi/");

        // Pause
        private void Button4_Click(object sender, EventArgs e)
        {
            switch (button4.Text)
            {
                case "Pause":
                    takingRequests = false;
                    twitchClient.SendMessage(mainChannel,
                        userSettings.Default.requestPause.Parse(channel: mainChannel.Channel));
                    button4.Text = @"Unpause";
                    break;
                case "Unpause":
                    takingRequests = true;
                    twitchClient.SendMessage(mainChannel,
                        userSettings.Default.requestUnpause.Parse(channel: mainChannel.Channel));
                    button4.Text = @"Pause";
                    break;
            }
        }

        // Update
        private void Button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                @"The bot will open a download for the update and close itself."
                + Environment.NewLine
                + @"Delete everything in the bots folder and extract the new archive inside and restart the program.");

            Process.Start("http://g.whaskell.pw/requestbot/latest.zip");

            Environment.Exit(0);
        }

        // Settings
        private void Button6_Click(object sender, EventArgs e) => new SettingsForm().Show();

        // Alerts
        private void Button7_Click(object sender, EventArgs e)
        {
            if (!alertsOn)
            {
                if (string.IsNullOrEmpty(Settings.Default.slKey))
                {
                    MessageBox.Show(
                        $@"For donations to work, you need to enter your StreamLabs API key in the Window that will open.
                            {Environment.NewLine}"
                        + @"If you do not want donations alerts or don't use StreamLabs, leave this empty.");
                    var sl = new slForm();
                    sl.ShowDialog();
                }

                alertsOn = true;
                button7.Text = @"Disable Alerts";

                slClient = new Client();
                slClient.Connect(Settings.Default.slKey);

                slClient.OnTwitchFollow += OnTwitchFollow;
                slClient.OnTwitchHost += OnTwitchHost;
                slClient.OnTwitchSubscription += OnTwitchSub;
                slClient.OnDonation += OnDonation;
            }
            else
            {
                try
                {
                    slClient.Disconnect();
                }
                catch
                {
                    // ignored.
                }

                alertsOn = false;
                button7.Text = @"Enable Alerts";
            }
        }

        // Play
        private void Button8_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
                if(item.BackColor == Color.GreenYellow)
                    DelItem(item);

            twitchClient.SendMessage(mainChannel,
                $"Now playing: {listView1.SelectedItems[0].SubItems[0].Text}. Requested by: {listView1.SelectedItems[0].SubItems[1].Text}");

            listView1.SelectedItems[0].BackColor = Color.GreenYellow;
        }

        private void OnDisconnected(object sender, OnDisconnectedArgs e)
        {
            if (!userDisconnect)
            {
                LabelChange(@"Status: Reconnecting...");

                twitchClient.Connect();
            }
            else
            {
                LabelChange(@"Status: Disconnected");
            }
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            // !request
            if (e.ChatMessage.Message.StartsWith($"{userSettings.Default.requestCommand} ", StringComparison.Ordinal) &&
                e.ChatMessage.Channel == mainChannel.Channel)
            {
                var user = e.ChatMessage.Username;
                var song = e.ChatMessage.Message.Replace($"{userSettings.Default.requestCommand} ", "");

                if (takingRequests)
                    if (HasRequest(user, out var row))
                    {
                        ChangeSong(row, song);
                        twitchClient.SendMessage(e.ChatMessage.Channel,
                            userSettings.Default.requestChanged.Parse(e.ChatMessage.Username, mainChannel.Channel,
                                (row + 1).ToString()));
                    }
                    else
                    {
                        var newItem = new ListViewItem(song);
                        newItem.SubItems.Add(user);

                        AddItem(newItem);

                        twitchClient.SendMessage(e.ChatMessage.Channel,
                            userSettings.Default.requestAdded.Parse(e.ChatMessage.Username, mainChannel.Channel,
                                listView1.Items.Count.ToString()));
                    }
                else
                    twitchClient.SendMessage(e.ChatMessage.Channel,
                        userSettings.Default.requestDisabled.Parse(e.ChatMessage.Username, mainChannel.Channel));
            }

            // !remove (mod)
            if (e.ChatMessage.Message.StartsWith($"{userSettings.Default.removeCommand} ", StringComparison.Ordinal) &&
                e.ChatMessage.Channel == mainChannel.Channel && e.ChatMessage.IsMod())
            {
                var targetUser = e.ChatMessage.Message.Replace($"{userSettings.Default.removeCommand} ", "");

                if (HasRequest(targetUser, out var row))
                {
                    DelSongRow(row);
                    twitchClient.SendMessage(mainChannel,
                        userSettings.Default.requestRemoved.Parse(targetUser, mainChannel.Channel, row.ToString()));
                }
            }

            // !remove (self)
            if (e.ChatMessage.Message.Equals(userSettings.Default.removeCommand) ||
                e.ChatMessage.Message.Equals($"{userSettings.Default.removeCommand} "))
                if (HasRequest(e.ChatMessage.Username, out var row))
                {
                    DelSongRow(row);
                    twitchClient.SendMessage(mainChannel,
                        userSettings.Default.requestRemoved.Parse(e.ChatMessage.Username, mainChannel.Channel,
                            row.ToString()));
                }

            // !info
            if (e.ChatMessage.Message.Equals("!info") && e.ChatMessage.IsMod())
            {
                var response = string.Empty;

                response += $"LeafyDev's bot version {version}. | ";
                response += $"Songs in queue: {listView1.Items.Count} | ";
                response +=
                    $"Bot has been running for: {DateTime.Now - Process.GetCurrentProcess().StartTime:hh\\:mm\\:ss}";

                var rand = new Random();
                const int iterations = 10000000;
                var sum = Enumerable.Range(1, iterations)
                    .Count(i => rand.Next(1, 101) <= 25);
                if (sum / (float) iterations * 1000 - 250 < 0) response += " | Broadcaster is hydrated: false Kappa";

                twitchClient.SendMessage(mainChannel, response);
            }
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            LabelChange($@"Status: Connected to {e.Channel}");

            mainChannel = twitchClient.JoinedChannels[0];

            twitchClient.SendMessage(e.Channel,
                userSettings.Default.requestConnected.Parse(channel: mainChannel.Channel));
        }

        private bool HasRequest(string user, out int row)
        {
            var result = false;

            listView1.Invoke(new Action(() =>
            {
                foreach (ListViewItem item in listView1.Items)
                    if (item.SubItems[1].Text == user)
                    {
                        if (item.BackColor == Color.GreenYellow)
                        {
                            result = false;
                            break;
                        }

                        tmpRow = item.Index;
                        result = true;
                        break;
                    }
            }));

            row = tmpRow;
            tmpRow = 0;
            return result;
        }

        private void AddItem(ListViewItem itemToAdd)
        {
            if (InvokeRequired)
            {
                Invoke(new AddItemInvoker(AddItem), itemToAdd);
                return;
            }

            listView1.Items.Add(itemToAdd);
        }

        private void DelItem(ListViewItem itemToDel)
        {
            if (InvokeRequired)
            {
                Invoke(new DelItemInvoker(DelItem), itemToDel);
                return;
            }

            listView1.Items.Remove(itemToDel);
        }

        private void LabelChange(string newText)
        {
            if (InvokeRequired)
            {
                Invoke(new LabelInvoker(LabelChange), newText);
                return;
            }

            label2.Text = newText;
        }

        private void ChangeSong(int _row, string _text)
        {
            if (InvokeRequired)
            {
                Invoke(new ChangeSongInvoker(ChangeSong), _row, _text);
                return;
            }

            listView1.Items[_row].SubItems[0].Text = _text;
        }

        private void DelSongRow(int _row)
        {
            if (InvokeRequired)
            {
                Invoke(new DelSongRowInvoker(DelSongRow), _row);
                return;
            }

            DelItem(listView1.Items[_row]);
        }

        private void OnTwitchFollow(object sender, StreamlabsEvent<TwitchFollowMessage> e) =>
            twitchClient.SendMessage(mainChannel, userSettings.Default.newFollow.Parse(e.Message[0].Name));

        private void OnDonation(object sender, StreamlabsEvent<DonationMessage> e) => twitchClient.SendMessage(
            mainChannel,
            userSettings.Default.newDonation.Parse(e.Message[0].From,
                amount: $"{e.Message[0].Amount}{e.Message[0].Currency}"));

        private void OnTwitchSub(object sender, StreamlabsEvent<TwitchSubscriptionMessage> e)
        {
            if (e.Message[0].Months == 1)
                twitchClient.SendMessage(mainChannel,
                    userSettings.Default.newSub.Parse(e.Message[0].Name, tier: e.Message[0].SubType));
            else
                twitchClient.SendMessage(mainChannel,
                    userSettings.Default.newResub.Parse(e.Message[0].Name, amount: e.Message[0].Months.ToString(),
                        tier: e.Message[0].SubType));
        }

        private void OnTwitchHost(object sender, StreamlabsEvent<TwitchHostMessage> e) => twitchClient.SendMessage(
            mainChannel,
            userSettings.Default.newHost.Parse(e.Message[0].Name, amount: e.Message[0].Viewers));

        private delegate void AddItemInvoker(ListViewItem itemToAdd);

        private delegate void DelItemInvoker(ListViewItem itemToDel);

        private delegate void LabelInvoker(string newText);

        private delegate void ChangeSongInvoker(int _row, string _text);

        private delegate void DelSongRowInvoker(int _row);
    }
}