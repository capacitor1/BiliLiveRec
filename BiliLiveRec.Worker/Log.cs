using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BiliLiveRec.Worker
{
    internal class Log
    {
        /*
         * 日志功能说明
         * 1. 所有日志以CL开头
         * 2. 所有日志均以|管道符分隔，不得在消息内部出现额外的管道符。
         * 
         * 程序处理时先检测是否是CL开头，然后用Split分割。
         * [0] = CL
         * [1] = RoomID
         * [2] = 操作标识符
         * MESSAGE为打印消息，此时[3]为消息内容文本；
         * STATUS_CHANGE为状态变化，此时[3]为状态（IDLE为没有任务（通用状态），REC为开始录制，STOP为正在暂停录制，PAUSE为已暂停），[4]为消息文本。
         * STAT为统计信息输出。
         */
        public static void LogInfo(string message,string id) => Console.WriteLine($"CL|{id}|{message}");
        public static void LogError(string message, string id) => Console.Error.WriteLine($"CL|{id}|{message}");
    }
    internal static class LogExt
    {
        public static void Log(this StreamWriter sw,string logmsg)
        {
            sw.WriteLine($"[{DateTime.Now}] {logmsg}");
            sw.Flush();
        }
    }
}
