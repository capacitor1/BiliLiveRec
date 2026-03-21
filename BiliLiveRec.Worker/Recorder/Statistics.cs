using System;
using System.Collections.Generic;
using System.Text;

namespace BiliLiveRec.Worker.Recorder
{
    internal class Statistics
    {
        public static string GetStatisticsString(string roomid,string m3u8url,string recpath,string title,TimeSpan t,long dlbytes,long crc32fail,long dlpieces,long segs)
        {
            return $"""
                ---------------BiliLiveRecorder Record Statistics---------------

                Overall
                ----------------------------------------------------------------
                Physical Path = '{recpath}'
                Format = 'FMP4'
                Version = 7

                Room
                ----------------------------------------------------------------
                Id = {roomid}
                Title = '{title}'
                HLS Url = '{m3u8url}'

                Record
                ----------------------------------------------------------------
                Time Elapsed = {t}
                Total Pieces = {dlpieces}
                Total Video Segments = {segs}
                Total Bytes = {dlbytes} ({FormatBytes(dlbytes)})
                Crc32 Failed = {crc32fail}

                VideoList
                ----------------------------------------------------------------
                {GetSegList(segs)}
                """;
        }
        public static string GetTmpStatisticsString(string roomid, string m3u8url, string recpath, string title)
        {
            return $"""
                ---------------BiliLiveRecorder Record Statistics---------------

                Overall
                ----------------------------------------------------------------
                Physical Path = '{recpath}'
                Format = 'FMP4'
                Version = 7

                Room
                ----------------------------------------------------------------
                Id = {roomid}
                Title = '{title}'
                HLS Url = '{m3u8url}'

                """;
        }
        private static string FormatBytes(double bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB" };
            int i;
            for (i = 0; i < suffixes.Length && bytes >= 1024; i++)
            {
                bytes /= 1024;
            }
            return $"{bytes:0.##} {suffixes[i]}";
        }
        private static string GetSegList(long segcount)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0;i < segcount; i++)
            {
                sb.AppendLine($"Video.{i:D5}.mp4");
            }
            return sb.ToString();
        }
    }
}
