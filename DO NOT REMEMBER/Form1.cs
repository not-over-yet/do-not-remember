using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DO_NOT_REMEMBER
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        private string[] buffer;

        private string path,
                       watermark,
                       exception,
                       procName;

        private bool isAuto,
                     isExec;

        private int count;

        private Dictionary<string, Resource> resource;
        private Uri uri;

        public Form1()
        {
            InitializeComponent();

            Resource();
            Parse(true);

            Detector.inputCallBack += InputCallBack;
            Detector.bindShortcutCallBack += BindShortcutCallBack;
            Detector.changeShortcutCallBack += ChangeShortcutCallBack;
            Detector.executeShortcutCallBack += ExecuteShortcutCallBack;

            Detector.ListenForEvents();
        }

        #region Controll Event Methods
        private void metroButton1_Click(object sender, EventArgs e)
        {
            Bind();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            if (Parse(true))
                Execute();
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text.Length == 0)
            {
                textBox2.Text = watermark;
                textBox2.ForeColor = Color.LightGray;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == watermark)
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.MediumSpringGreen;
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                if (Parse(true))
                    Execute();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            Visible = ShowInTaskbar = false;
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Visible = ShowInTaskbar = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;

            Application.ExitThread();
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visible = ShowInTaskbar = true;
        }
        #endregion

        #region Call-Back Methods
        private void InputCallBack(bool button)
        {
            if (button) // left button
            {
                if (isExec)
                {
                    isExec = false;
                    count = 2;

                    notifyIcon1.BalloonTipTitle = "CANCEL ";
                    notifyIcon1.BalloonTipText = "\n" + procName;
                    notifyIcon1.ShowBalloonTip(100);
                }
            }
            else if (isExec)
            {
                try
                {
                    Process.Start(uri.AbsoluteUri);

                    isExec = false;
                    isAuto = true;

                    listBox1.SelectedItem = procName;

                    Bind();
                }
                catch
                {
                    isExec = isAuto = false;
                }
            }
            else if (count < 2)
            {
                if (isAuto)
                {
                    Thread.Sleep(50);

                    Clipboard.SetText(buffer[count++]);

                    SendKeys.SendWait("^{v}");
                    SendKeys.SendWait("{TAB}");

                    Thread.Sleep(50);

                    Clipboard.SetText(buffer[count++]);

                    SendKeys.SendWait("^{v}");
                    SendKeys.SendWait("{ENTER}");

                    Thread.Sleep(50);
                }
                else if (!string.IsNullOrWhiteSpace(buffer[count]))
                {
                    Thread.Sleep(50);

                    Clipboard.SetText(buffer[count++]);
                    SendKeys.SendWait("^{v}");

                    Thread.Sleep(50);
                }

                Clipboard.Clear();
            }
        }

        private void BindShortcutCallBack(int idx, bool isAuto)
        {
            if (listBox1.Items.Count > idx)
            {
                listBox1.SelectedItem = listBox1.Items[idx];
                this.isAuto = isAuto;
                Bind();
            }
        }

        private void ExecuteShortcutCallBack(int idx)
        {
            if (listBox1.Items.Count > idx)
            {
                var key =
                    listBox1.Items[idx].ToString();

                listBox1.SelectedItem = key;

                if (Uri.TryCreate(resource[key].pth, UriKind.Absolute, out uri))
                {
                    isExec = true;
                    procName = key;

                    notifyIcon1.BalloonTipTitle = "RUN? ";
                    notifyIcon1.BalloonTipText = "\n" + procName;
                    notifyIcon1.ShowBalloonTip(100);
                }
            }
        }

        private void ChangeShortcutCallBack(int idx)
        {
            if (listBox1.Items.Count > idx)
            {
                var key =
                    listBox1.Items[idx].ToString();

                listBox1.SelectedItem = key;

                resource[key].pw = Change(resource[key].pw);

                Mapping();

                notifyIcon1.BalloonTipTitle = "CHANGE! ";
                notifyIcon1.BalloonTipText = "\n" + key;
                notifyIcon1.ShowBalloonTip(100);
            }
        }
        #endregion

        #region Custom Methods
        private void Bind()
        {
            if (listBox1.SelectedItem != null)
            {
                isExec = false;

                var key =
                    listBox1.SelectedItem.ToString();

                if (key != exception)
                {
                    count = 0;

                    buffer[0] = new string(resource[key].id.ToCharArray());
                    buffer[1] = new string(resource[key].pw.ToCharArray());

                    notifyIcon1.BalloonTipTitle = "BIND! ";
                    notifyIcon1.BalloonTipText = "\n" + key;
                    notifyIcon1.ShowBalloonTip(100);
                }
            }
        }

        private bool Parse(bool toggle)
        {
            listBox1.Items.Clear();

            try
            {
                if (toggle)
                    resource = 
                        JsonConvert.DeserializeObject<Dictionary<string, Resource>>(textBox1.Text);

                if (resource != null)
                    foreach (var e in resource.OrderBy(e => int.Parse(e.Value.ord)))
                        listBox1.Items.Add(e.Key);
            }
            catch
            {
                listBox1.Items.Clear();
                listBox1.Items.Add(exception);
                return false;
            }

            return true;
        }

        private void Mapping()
        {
            var obj =
                    JsonConvert.DeserializeObject(
                        JsonConvert.SerializeObject(resource)
                    ).ToString();

            textBox1.Text = obj;
            File.WriteAllText(path, obj);
        }

        private bool Judge(ref string[] values, string param = "")
        {
            if (param.Length < 4 || param[3] != ' ')
                return false;

            var buf = param.Split(' ');
            var stb = new StringBuilder();

            for (int i = 1; i < buf.Length; i++)
                stb.Append(i > 1 ? string.Format(" {0}", buf[i]) : buf[i]);
           
            var exe = values[0] = buf[0];
            var data = stb.ToString().Split('<');

            switch (exe)
            {
                case "ins":
                    if (data.Length != 5)
                        return false;

                    for (int i = 1; i < 6; i++)
                        values[i] = data[i - 1];

                    break;
                case "upd":
                    if (data.Length != 3)
                        return false;

                    for (int i = 1; i < 4; i++)
                        values[i] = data[i - 1];

                    break;
                case "del":
                    if (data.Length != 1)
                        return false;

                    values[1] = data[0];

                    break;
                default:
                    return false;
            }

            return true;
        }

        private void Execute()
        {
            if (textBox2.Text != watermark)
            {
                var buf = new string[6];

                if (Judge(ref buf, textBox2.Text))
                {
                    var exe = buf[0];
                    int num;

                    switch (exe)
                    {
                        case "ins":
                            if (!resource.ContainsKey(buf[1]))
                            {
                                resource[buf[1]] = new Resource()
                                {
                                    id = buf[2],
                                    pw = buf[3],
                                    ord = int.TryParse(buf[4], out num) ? buf[4] : "99",
                                    pth = buf[5]
                                };
                            }
                            break;
                        case "upd":
                            if (resource.ContainsKey(buf[1]))
                            {
                                resource[buf[1]].pw = buf[2];
                                resource[buf[1]].ord = int.TryParse(buf[3], out num) ? buf[3] : "99";
                            }
                            break;
                        case "del":
                            if (resource.ContainsKey(buf[1]))
                                resource.Remove(buf[1]);
                            break;
                    }

                    textBox2.Text = "";
                }
            }

            Mapping();
            Parse(false);
        }

        private void Resource()
        {
            path = string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), "resource.json");

            if (!File.Exists(path))
                File.Create(path);

            buffer = new string[2];

            exception = "EXCEPTION :(";

            textBox1.Text = File.ReadAllText(path);
            textBox2.Text = watermark = "command line";
            textBox2.ForeColor = Color.LightGray;
        }

        private string Change(string password)
        {
            var arr = password.ToCharArray();

            if (NextPermutation(arr))
                return new string(arr);

            int idx = 0;
            var buf = new char[arr.Length];

            foreach (var j in arr.Reverse())
                buf[idx++] = j;

            return new string(buf);
        }

        private bool NextPermutation(char[] arr)
        {
            int i = arr.Length - 1;
            while (i > 0 && arr[i - 1] >= arr[i])
                i--;

            if (i <= 0) return false;

            int j = arr.Length - 1;
            while (arr[j] <= arr[i - 1])
                j--;

            char temp = arr[i - 1];
            arr[i - 1] = arr[j];
            arr[j] = temp;

            j = arr.Length - 1;
            while (i < j)
            {
                temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
                i++;
                j--;
            }

            return true;
        }
        #endregion
    }
}
