using BiliLiveRec.Worker;
using BiliLiveRec.Worker.Network;
using BiliLiveRec.Worker.Recorder;
using System.Text.Json.Nodes;

//全局变量
Http.Init();
string _RoomId = string.Empty;
string _BasePath = string.Empty;
int _Waiting = 30000;
bool _IsRec = false,_CanRec = true;

//判断命令行输入
if (args.Length < 1)
{
    Log.LogError("MESSAGE|Missing RoomId.(-1001)","-1");
    Environment.Exit(-1001);
}
else if(args.Length == 1)
{
    _RoomId = args[0];
    _BasePath = Directory.GetCurrentDirectory();
}
else if(args.Length == 2)
{
    _RoomId = args[0]; 
    _BasePath = args[1];
}
else if(args.Length > 2)
{
    Log.LogInfo($"MESSAGE|Unknown arguments : {String.Join(',', args[2..])}","-1");
}
//获取房间基本信息


//创建文件夹
Directory.CreateDirectory(Path.Combine(_BasePath, _RoomId));
//日志
StreamWriter _LogInstance = new StreamWriter(Path.Combine(_BasePath,_RoomId + ".log"),true);

_LogInstance.Log($"Start recorder successfully with args : {String.Join(',', args)}");

//循环监控房间状态（API）
Log.LogInfo("STATUS_CHANGE|IDLE|Everything is OK.Monitoring...", _RoomId);
bool _IsLivingByAPI = false;
Task run = Task.Run(() => {
    while (true)
    {
        if (!_CanRec)
        {
            Thread.Sleep(_Waiting);
            continue;
        }
        //房间是否正在直播
        string wbi = WBI.GetWBIByID(_RoomId).Result;
        JsonNode ilfromapi = JsonNode.Parse(Http.GetString(Endpoint.Info + wbi).Result)!;
        if ((int)ilfromapi["code"]! != 0)
        {
            Log.LogError($"MESSAGE|Cannot get living status : {(string)ilfromapi["message"]!} ({(int)ilfromapi["code"]!})", _RoomId);
            _LogInstance.Log($"Cannot get living status : {(string)ilfromapi["message"]!} ({(int)ilfromapi["code"]!})");
            
            //等待后无限重试
            Thread.Sleep(1000);
            continue;
        }
        _IsLivingByAPI = (int)ilfromapi["data"]!["live_status"]! == 1;

        //没直播就返回去
        if (!_IsLivingByAPI)
        {
            Thread.Sleep(_Waiting);
            continue;
        }
        //直播：启动录制
        string _RecPath = Path.Combine(_BasePath,_RoomId, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ffff"));
        Log.LogInfo($"STATUS_CHANGE|REC|Start recording '{_RecPath}'", _RoomId);
        _LogInstance.Log($"Start recording '{_RecPath}'");

        Recorder.Record(_RoomId,_RecPath,_LogInstance,ref _IsRec,ref _CanRec);
        //录制完毕，继续监控
        Thread.Sleep(_Waiting);
        continue;
    }
});

//防止退出，输入exit退出
while (true)
{
    /*
     * 控制台输入说明
     * 1. 均以OPERATE开头，|管道符分割。
     * [1] 为操作码，
     * EXIT=退出
     * PAUSE=暂停录制
     * RESUME=继续录制
     */
    string input = Console.ReadLine()!;
    if (input == "OPERATE|EXIT")
    {
        if (_IsRec)
        {
            Log.LogInfo($"STATUS_CHANGE|STOP|Stopping recording task...", _RoomId);
            _LogInstance.Log($"Stopping recording task(exit)...");
            _CanRec = false;
            while(_IsRec) Thread.Sleep(1000);
        }
        Log.LogInfo($"MESSAGE|Exiting...", _RoomId);
        _LogInstance.Log($"Application Exit.(0)");
        _LogInstance.Dispose();
        Thread.Sleep(100);
        Environment.Exit(0);
    }
    else if(input == "OPERATE|PAUSE" && _CanRec)
    {
        Log.LogInfo($"STATUS_CHANGE|STOP|Stopping recording task...", _RoomId);
        _LogInstance.Log($"Stopping recording task(pause)...");
        _CanRec = false;
        //检测是否正在录制
        while (_IsRec) Thread.Sleep(1000);
        Log.LogInfo($"STATUS_CHANGE|PAUSE|Task paused.", _RoomId);
        _LogInstance.Log($"Task paused.");
    }
    else if (input == "OPERATE|RESUME" && !_CanRec)
    {
        Log.LogInfo($"STATUS_CHANGE|IDLE|Resuming recording task...", _RoomId);
        _LogInstance.Log($"Resumeing recording task...");
        _CanRec = true;
    }
}