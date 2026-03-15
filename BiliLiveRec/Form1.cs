using BiliLiveRec.Network;
using Microsoft.VisualBasic.Logging;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Nodes;

namespace BiliLiveRec
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Http.Init();
            //UI统计数据更新
            System.Timers.Timer t = new System.Timers.Timer(1000);
            t.Elapsed += new System.Timers.ElapsedEventHandler(UpdateStatView!);
            t.AutoReset = true;
            t.Enabled = true;
        }
        public readonly string Version = "1.0.0";
        public Dictionary<string, Worker> Rooms = new Dictionary<string, Worker>();
        /*
         * Value = 状态
         * 空：未启动
         * IDLE为没有任务（通用状态），REC为开始录制，STOP为正在暂停录制，PAUSE为已暂停
         * 图标：未启动0 IDLE1 REC2 STOP3 PAUSE4
         * 
         */
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = $"BiliLiveRec v{Version}";
            UI_LogView.Info($"BiliLiveRec v{Version}", "App.Init");

            //设置输出文件夹
            if (!File.Exists(RecPathFile)) File.WriteAllText(RecPathFile, RecPath);
            RecPath = File.ReadAllText(RecPathFile).Trim('"');
            UI_LogView.Info($"Current RecPath is '{Path.GetFullPath(RecPath)}'", "App.Init");
            //加载房间
            if (File.Exists(RoomIdFile))
            {
                foreach (var r in File.ReadAllLines(RoomIdFile))
                {
                    Worker w = new Worker(r, RecPath);
                    w.OutputDataReceived += W_OutputDataReceived;
                    w.ErrorDataReceived += W_ErrorDataReceived;
                    Rooms.Add(r, w);
                }
                RefreshRoomsView();
            }
        }

        private void W_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.Data)) return;

                ProcessWOutput(e.Data.Split('|'), false);
            }
            catch (Exception ex)
            {
                UI_LogView.Warn($"Exception at processing cmd output line : {ex.Message}", "App.CmdRecv");
            }

        }

        private void W_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.Data)) return;

                ProcessWOutput(e.Data.Split('|'), true);
            }
            catch (Exception ex)
            {
                UI_LogView.Warn($"Exception at processing cmd output line : {ex.Message}", "App.CmdRecv");
            }
        }
        public void ProcessWOutput(string[] raw, bool isinfo)
        {
            if (raw[0] != "CL") return;
            switch (raw[2])
            {
                case "MESSAGE":
                    if (!isinfo) UI_LogView.Error(raw[3], raw[1]);
                    else UI_LogView.Info(raw[3], raw[1]);
                    break;
                case "STATUS_CHANGE":
                    UI_LogView.InfoGreen($"Status changed : {raw[3]} , {raw[4]}", raw[1]);
                    RefreshRoomsView();
                    break;
                case "STAT":
                    STAT_DownloadedBytes += long.Parse(raw[3]);
                    STAT_Pieces += long.Parse(raw[4]);
                    break;
                default:
                    UI_LogView.Warn($"Unknown worker returned line '{String.Join('|', raw[1..])}'", raw[1]);
                    break;
            }
        }
        void UpdateStatView(object source, System.Timers.ElapsedEventArgs e)
        {
            lock (_lockObj2)
            {
                UI_StatView.Text = $"已启动：{Rooms.Values.Count(w => w.WorkerState != Worker.State.NONE)} 活动：{Rooms.Values.Count(w => w.WorkerState == Worker.State.REC)} | 已下载： {STAT_Pieces}（{STAT_DownloadedBytes:N0} 字节）";
            }
        }
        public long STAT_DownloadedBytes = 0, STAT_Pieces = 0;

        public string RoomIdFile = "RoomIDs.lst";
        public string RecPathFile = "RecPath.txt";

        public string RecPath = @"Recorded";//默认路径

        private void UI_Rooms_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && UI_Rooms.SelectedItems.Count > 0)
            {
                //用户弹窗提示
                DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete {UI_Rooms.SelectedItems.Count} room(s)?",
                "Delete Rooms",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
                );
                if (result == DialogResult.Yes)
                {
                    foreach (ListViewItem ro in UI_Rooms.SelectedItems)
                    {
                        if (ro.ImageIndex == 0)
                        {
                            //直接删
                            Rooms.Remove(ro.Text);
                        }
                        else
                        {
                            //程序在运行，先给程序发送退出指示，然后等待程序状态改变时删掉
                            Rooms[ro.Text].Exit();
                            //
                            Rooms.Remove(ro.Text);
                        }
                        UI_Rooms.Items.Remove(ro);
                        UI_LogView.Info($"Removed room {ro.Text} !", "App.Main");
                    }
                    if (File.Exists(RoomIdFile)) { File.Delete(RoomIdFile); }
                    File.WriteAllLines(RoomIdFile, Rooms.Keys);
                    RefreshRoomsView();
                }
            }
            else if (e.KeyCode == Keys.F5)
            {
                RefreshRoomsView();
                Refresh();
            }
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UI_LogView.Text = string.Empty;
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Log File (*.log)|*.log";
                saveFileDialog.Title = "Export Log";
                saveFileDialog.FileName = $"Log_{DateTime.Now:yyyy_MM_dd_HH_mm_ss_ffff}";
                saveFileDialog.DefaultExt = "log";
                saveFileDialog.AddExtension = true;

                // 如果用户点击了"保存"按钮
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(saveFileDialog.FileName, UI_LogView.Text, System.Text.Encoding.UTF8);
                        UI_LogView.Info($"Log exported : {saveFileDialog.FileName}", "App.Log");
                    }
                    catch (Exception ex)
                    {
                        UI_LogView.Error($"Log export failed:\r\n{ex.Message}", "App.Log");
                    }
                }
            }
        }

        private async void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //获取输入
            Input input = new Input();
            string res = input.GetResult();

            string _rawid = string.Empty;
            //校验
            if (res == string.Empty)
            {
                return;
            }
            if (long.TryParse(res, out long rawid))
            {
                _rawid = rawid.ToString();
            }
            else if (res.StartsWith("https://live.bilibili.com/", StringComparison.OrdinalIgnoreCase))
            {
                _rawid = res[26..];
                if (_rawid.Contains('?'))
                {
                    _rawid = _rawid.Split('?')[0];
                }

                //最后检查
                if (!long.TryParse(_rawid, out _))
                {
                    _rawid = string.Empty;
                }
            }
            else
            {
                MessageBox.Show("Invalid room ID, please check.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //最终校验
            if (_rawid == string.Empty)
            {
                UI_LogView.Warn("Invalid or empty room ID, please check.", "App.Input");
                return;
            }

            //获取真实ID
            try
            {
                string wbi = await WBI.GetWBIByID(_rawid);
                JsonNode idfromapi = JsonNode.Parse(await Http.GetString(Endpoint.Info + wbi))!;
                if ((int)idfromapi["code"]! != 0)
                {
                    UI_LogView.Error($"Cannot verify RoomId : {(string)idfromapi["message"]!} ({(int)idfromapi["code"]!})", "App.Input");
                    return;
                }
                string apiid = idfromapi["data"]!["room_id"]!.ToString();
                if (apiid != _rawid)
                {
                    UI_LogView.Info($"Checked room id {_rawid} --> {apiid}", "App.Input");
                }
                _rawid = apiid;

            }
            catch (Exception ex)
            {
                UI_LogView.Error($"Can not add room : {ex.Message}", "App.Input");
                return;
            }

            //添加房间
            //todo：添加用户头像
            if (!Rooms.ContainsKey(_rawid))
            {
                Worker w = new Worker(_rawid, RecPath);
                w.OutputDataReceived += W_OutputDataReceived;
                w.ErrorDataReceived += W_ErrorDataReceived;
                Rooms.Add(_rawid, w);
                UI_LogView.Info($"Added room ID {_rawid}.", "App.Input");

                //结束
                if (File.Exists(RoomIdFile)) { File.Delete(RoomIdFile); }
                File.WriteAllLines(RoomIdFile, Rooms.Keys);

                RefreshRoomsView();
            }
            else
            {
                UI_LogView.Warn($"Ignoring duplicate room ID {_rawid}.", "App.Input");
            }

        }
        private static readonly Lock _lockObj = new Lock();
        private static readonly Lock _lockObj2 = new Lock();
        public void RefreshRoomsView()
        {
            lock (_lockObj)
            {
                //
                UI_Rooms.Items.Clear();
                foreach (var room in Rooms)
                {
                    UI_Rooms.Items.Add(room.Key, (int)room.Value.WorkerState);
                }
            }
        }

        private async void StartRecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //全部启动
            foreach (var room in Rooms)
            {
                room.Value.Start();
                UI_LogView.Info($"Send signal : START --> {room.Key}", "App.Worker");
                await Task.Delay(200);
            }
        }

        private void RecPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //修改输出文件夹
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.ShowNewFolderButton = true;
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(RecPathFile)) File.Delete(RecPathFile);

                    File.WriteAllText(RecPathFile, folderDialog.SelectedPath);
                    RecPath = folderDialog.SelectedPath;
                    UI_LogView.Info($"Current RecPath is '{Path.GetFullPath(RecPath)}' (This will take effect after restart record worker.)", "App.Main");
                    foreach (var room in Rooms)
                    {
                        room.Value.UpdateRecPath(RecPath);
                    }
                }
            }
        }

        private void StopRecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //全部停止
            foreach (var room in Rooms)
            {
                room.Value.Pause();
                UI_LogView.Info($"Send signal : PAUSE --> {room.Key}", "App.Worker");
            }
        }

        private void StopRec1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //全部停止
            foreach (var room in Rooms)
            {
                room.Value.Exit();
                UI_LogView.Info($"Send signal : EXIT --> {room.Key}", "App.Worker");
            }
            RefreshRoomsView();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Rooms.Values.Any(w => w.WorkerState != Worker.State.NONE))
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure to EXIT?",
                    "EXIT",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    foreach (var room in Rooms)
                    {
                        room.Value.Kill();
                    }
                    //Exit application
                }
            }
            //Exit application
        }

        private async void StartRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UI_Rooms.SelectedItems.Count > 0)
            {
                foreach (ListViewItem r in UI_Rooms.SelectedItems)
                {
                    if (Rooms[r.Text].WorkerState == Worker.State.IDLE || Rooms[r.Text].WorkerState == Worker.State.REC) continue;
                    Rooms[r.Text].Start();
                    UI_LogView.Info($"Send signal : START --> {r.Text}", "App.Worker");
                    await Task.Delay(200);
                }
            }
        }

        private void StopRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UI_Rooms.SelectedItems.Count > 0)
            {
                foreach (ListViewItem r in UI_Rooms.SelectedItems)
                {
                    if (Rooms[r.Text].WorkerState == Worker.State.PAUSE) continue;
                    Rooms[r.Text].Pause();
                    UI_LogView.Info($"Send signal : PAUSE --> {r.Text}", "App.Worker");
                }
            }
        }

        private void StopR1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UI_Rooms.SelectedItems.Count > 0)
            {
                foreach (ListViewItem r in UI_Rooms.SelectedItems)
                {
                    Rooms[r.Text].Exit();
                    UI_LogView.Info($"Send signal : EXIT --> {r.Text}", "App.Worker");
                }
                RefreshRoomsView();
            }
        }

        private void UI_LogView_TextChanged(object sender, EventArgs e)
        {
            UI_LogView.ScrollToCaret();
        }

        private void OpenURL_Click(object sender, EventArgs e)
        {
            if (UI_Rooms.SelectedItems.Count > 0)
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = $"https://live.bilibili.com/{UI_Rooms.SelectedItems[0].Text}",
                    UseShellExecute = true
                });
            }
        }

        private void CopyRIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (UI_Rooms.SelectedItems.Count > 0)
                {
                    string ids = string.Empty;
                    foreach (ListViewItem r in UI_Rooms.SelectedItems) ids += $"{r.Text},";
                    Clipboard.SetDataObject(ids.TrimEnd(','), true, 10, 200);
                }
            }
            catch (Exception ex)
            {
                UI_LogView.Error($"Clipboard error : {ex.Message}", "App.ClipBoard");
            }
        }

        private void ForceExit_Click(object sender, EventArgs e)
        {
            if (UI_Rooms.SelectedItems.Count > 0)
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure to FORCE STOP?\r\nThis may cause data loss or corruption.",
                    "Force stop",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    foreach (ListViewItem r in UI_Rooms.SelectedItems)
                    {
                        if (Rooms[r.Text].WorkerState == Worker.State.NONE) continue;
                        Rooms[r.Text].Kill();
                        UI_LogView.Info($"FORCE STOP --> {r.Text}", "App.Worker");
                    }
                }
            }
        }
    }
}
