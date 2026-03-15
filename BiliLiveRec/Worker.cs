using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Text;

namespace BiliLiveRec
{
    public class Worker
    {
        private readonly string _id;
        private string _path;
        private ProcessStartInfo _psi;
        private Process? _cmdProcess;
        public State WorkerState { get; private set; }
        public enum State
        {
            NONE = 0,
            IDLE = 1,
            REC = 2,
            STOP = 3,
            PAUSE = 4
        }
        public Worker(string RoomID, string RecBasePath)
        {
            _id = RoomID;
            _path = RecBasePath;
            WorkerState = State.NONE;
            _psi = new ProcessStartInfo
            {
                FileName = "BiliLiveRec.Worker.exe",
                Arguments = $"{_id} \"{_path}\"",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
        }
        public void UpdateRecPath(string path)
        {
            _path = path;
            _psi.Arguments = $"{_id} \"{_path}\"";
        }

        /// <summary>
        /// 进程标准输出事件
        /// </summary>
        public event DataReceivedEventHandler? OutputDataReceived;

        /// <summary>
        /// 进程错误输出事件
        /// </summary>
        public event DataReceivedEventHandler? ErrorDataReceived;
        public void Start()//启动（如果没有启动进程，启动。如果已启动，发送恢复信号。
        {
            if (_cmdProcess == null)
            {
                _cmdProcess = new Process { StartInfo = _psi, EnableRaisingEvents = true };
                _cmdProcess.OutputDataReceived += (sender, e) =>
                {
                    //解析状态
                    try
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            string[] raw = e.Data.Split('|');
                            if (raw[0] == "CL")
                            {
                                if (raw[2] == "STATUS_CHANGE")
                                {
                                    WorkerState = raw[3] switch
                                    {
                                        "IDLE" => State.IDLE,
                                        "REC" => State.REC,
                                        "STOP" => State.STOP,
                                        "PAUSE" => State.PAUSE,
                                        _ => State.NONE,
                                    };
                                }
                            }
                        }
                    }
                    catch
                    {
                        //
                    }
                    OutputDataReceived?.Invoke(sender, e);
                };
                _cmdProcess.ErrorDataReceived += (sender, e) => ErrorDataReceived?.Invoke(sender, e);
                _cmdProcess.Exited += (sender, e) =>
                {
                    WorkerState = State.NONE;//退出处理
                    _cmdProcess.Dispose();
                    _cmdProcess = null;
                };
                _cmdProcess.Start();
                _cmdProcess.BeginOutputReadLine();
                _cmdProcess.BeginErrorReadLine();
            }
            else
            {
                _cmdProcess.StandardInput.WriteLine("OPERATE|RESUME");
            }
        }
        public void Pause()//暂停
        {
            _cmdProcess?.StandardInput.WriteLine("OPERATE|PAUSE");
        }
        public void Exit()//退出
        {
            _cmdProcess?.StandardInput.WriteLine("OPERATE|EXIT");
            _cmdProcess?.WaitForExit(1000 * 120);//等待120s
            if (_cmdProcess != null)
            {
                if(!_cmdProcess.HasExited) _cmdProcess?.Kill();
            }
            WorkerState = State.NONE;
            //等待停止后，清理
            _cmdProcess?.Dispose();
            _cmdProcess = null;
        }
        public void Kill()
        {
            _cmdProcess?.Kill();
            WorkerState = State.NONE;
            _cmdProcess?.Dispose();
            _cmdProcess = null;
        }
    }
}
