using BiliLiveRec.Worker.Network;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BiliLiveRec.Worker.Recorder
{
    internal class Recorder
    {
        private static readonly int MaxRetry = 10;
        public static void Record(string _RoomId, string _RecPath, StreamWriter _LogInstance, ref bool _IsRec, ref bool _CanRec)
        {
            _IsRec = true;
            bool _InternalCanRec;
            //启动录制
            DateTime _S_StartTime = DateTime.Now;
            Directory.CreateDirectory(_RecPath);
            //获取元数据
            string _S_Title;
        RetryInfo:
            try
            {
                string wbi = WBI.GetWBIByID(_RoomId).Result;
                JsonNode api = JsonNode.Parse(Http.GetString(Endpoint.Info + wbi).Result)!;
                if ((int)api["code"]! != 0)
                {
                    Log.LogError($"MESSAGE|Cannot get info : {(string)api["message"]!} ({(int)api["code"]!}).", _RoomId);
                    _LogInstance.Log($"Cannot get info : {(string)api["message"]!} ({(int)api["code"]!}).");
                    Thread.Sleep(1000);
                    goto RetryInfo;
                }
                File.WriteAllText(Path.Combine(_RecPath, "Info.json"), JsonSerializer.Serialize(api, Endpoint.Options));
                _S_Title = (string)api["data"]!["title"]!;
                _LogInstance.Log($"Downloaded Info.json");
                //封面
                Stream cstream = Http.GetStream((string)api["data"]!["user_cover"]!).Result;
                FileStream cover = File.OpenWrite(Path.Combine(_RecPath, "Cover.jpg"));
                cstream.CopyTo(cover);
                cstream.Dispose();
                cover.Dispose();
                _LogInstance.Log($"Downloaded Cover.jpg");
            }
            catch (Exception ex)
            {
                Log.LogError($"MESSAGE|Exception at get rec info : {ex.Message}", _RoomId);
                _LogInstance.Log($"Exception at get rec info : {ex.Message}");
                Thread.Sleep(1000);
                goto RetryInfo;
            }
            string _M3U8Url;
            //m3u8url
            //不稳定的方法，因为缺少WBI
        RetryStm:
            try
            {
                JsonNode json = JsonNode.Parse(Http.GetString(Endpoint.Stream + _RoomId).Result)!;
                if ((int)json["code"]! != 0)
                {
                    Log.LogError($"MESSAGE|Cannot get stream info : {(string)json["message"]!} ({(int)json["code"]!}).", _RoomId);
                    _LogInstance.Log($"Cannot get stream info : {(string)json["message"]!} ({(int)json["code"]!}).");
                    Thread.Sleep(1000);
                    goto RetryStm;
                }
                _M3U8Url = Utils.UGetUrl0(json);
                _LogInstance.Log($"Get m3u8 url successfully : {_M3U8Url}");
            }
            catch (Exception ex)
            {
                Log.LogError($"MESSAGE|Exception at get stream info : {ex.Message}", _RoomId);
                _LogInstance.Log($"Exception at get stream info : {ex.Message}");
                Thread.Sleep(1000);
                goto RetryStm;
            }

            //录制准备
            string _Host = _M3U8Url.Replace("index.m3u8", "");
            FileStream? _videowriter = null;
            MapWriter? _mapwriter = null;
            long _videoindex = 0, _PiecesPosition = -114514;
            MemoryStream _videowriter_tmp = new();
            //录制
            HttpResponseMessage? _M3U = Http.TryGetResp(_M3U8Url).Result;
            _InternalCanRec = _M3U.IsSuccessStatusCode;
            string[] _M3U8Content;
            List<string> _ExistsPieces = [];//已下载的分片

            //统计信息
            long _S_DownloadedBytes = 0, _S_Crc32FailedPieces = 0, Last_S_DownloadedBytes = 0, Last_ExistsPieces = 0;
            while (_CanRec && _InternalCanRec)
            {
                try
                {
                    //读取m3u8
                    _M3U8Content = _M3U.Content.ReadAsStringAsync().Result.Split('\n');
                    //解析m3u8并下载
                    foreach (string m3u8line in _M3U8Content)
                    {
                        if (m3u8line.StartsWith("#EXT-X-MAP"))
                        {
                            string mapfileurl = m3u8line.Replace("\"", "").Replace("#EXT-X-MAP:URI=", "");

                            //下载
                            if (!_ExistsPieces.Contains(mapfileurl))
                            {
                                _ExistsPieces.Add(mapfileurl);

                                //接到map，创建新的视频写入流
                                long endpos1 = _videowriter != null ? _videowriter.Position : 0;
                                _videowriter?.Dispose();
                                _videowriter = new(Path.Combine(_RecPath, $"Video.{_videoindex:D5}.mp4"),FileMode.OpenOrCreate,FileAccess.Write,FileShare.Read);
                                _LogInstance.Log($"Created video stream 'Video.{_videoindex:D5}.mp4'");
                                _videowriter.Position = _videowriter.Length;

                                _mapwriter?.Write($"#EXT-X-ENDLIST\r\n", endpos1, 0);
                                _mapwriter?.Close();
                                _mapwriter = new(Path.Combine(_RecPath, $"Video.{_videoindex:D5}.map"));
                                _LogInstance.Log($"Created video stream map 'Video.{_videoindex:D5}.map'");
                                _mapwriter!.Write($"#EXTM3U\r\n#EXT-X-VERSION:7\r\n#EXT-X-TARGETDURATION:0\r\n",0,0);
                                _videoindex++;

                                int retry = 1;
                                long pos = _videowriter.Position;
                            RetryMap:
                                try
                                {
                                    Stream map = Http.GetStream($"{_Host}{mapfileurl}").Result;
                                    _S_DownloadedBytes += map.Length;
                                    //
                                    map.CopyTo(_videowriter);
                                }
                                catch (Exception ex)
                                {
                                    _videowriter.Position = pos;//取消写入
                                    _LogInstance.Log($"Exception at get FMP4 map file '{mapfileurl}' : {ex.Message},retry {retry} / {MaxRetry}");
                                    Thread.Sleep(100);
                                    if (retry <= MaxRetry)
                                    {
                                        retry++;
                                        goto RetryMap;
                                    }
                                    else throw;
                                }
                                _mapwriter!.Write($"#EXT-X-DISCONTINUITY\r\n{m3u8line}\r\n", pos, _videowriter!.Position - pos);
                                _LogInstance.Log($"Get FMP4 map file '{mapfileurl}'");
                            }
                        }
                        else if (!m3u8line.Contains('#') && m3u8line.Contains(".m4s"))
                        {
                            long m4soffset = long.Parse(m3u8line.Replace(".m4s", string.Empty));
                            int listoffset = _M3U8Content.IndexOf(m3u8line);
                            //追赶之前的分片并整合
                            if (_PiecesPosition == -114514)
                            {
                                _PiecesPosition = m4soffset;
                                List<string> pm4s = [];
                                bool _IsExpired = false;
                                while (!_IsExpired)
                                {
                                    m4soffset--;
                                    //下载
                                    if (!_ExistsPieces.Contains($"{m4soffset}.m4s"))
                                    {
                                        _ExistsPieces.Add($"{m4soffset}.m4s");
                                        int retry = 1;
                                    Retry:
                                        try
                                        {
                                            HttpResponseMessage resp = Http.TryGetResp($"{_Host}{m4soffset}.m4s").Result;
                                            if (!resp.IsSuccessStatusCode)
                                            {
                                                _IsExpired = true;
                                                break;
                                            }
                                            //
                                            Stream stream = resp.Content.ReadAsStreamAsync().Result;
                                            _S_DownloadedBytes += stream.Length;
                                            Utils.WriteHttpStreamToFile(stream, Path.Combine(_RecPath, $"{m4soffset}.m4s_tmp"));
                                            pm4s.Add($"{m4soffset}.m4s");
                                        }
                                        catch (Exception ex)
                                        {
                                            _LogInstance.Log($"Exception at get previous m4s file '{m4soffset}' : {ex.Message},retry {retry} / {MaxRetry}");
                                            Thread.Sleep(100);
                                            if (retry <= MaxRetry)
                                            {
                                                retry++;
                                                goto Retry;
                                            }
                                            else throw;
                                        }
                                        _LogInstance.Log($"Get previous m4s file '{m4soffset}'");
                                        Log.LogInfo($"STAT|{_S_DownloadedBytes - Last_S_DownloadedBytes}|{_ExistsPieces.Count - Last_ExistsPieces}", _RoomId);
                                        Last_ExistsPieces = _ExistsPieces.Count;
                                        Last_S_DownloadedBytes = _S_DownloadedBytes;

                                    }
                                }
                                string[] pm4sl = [.. pm4s];
                                Array.Reverse(pm4sl);
                                foreach (string m4s in pm4sl)
                                {
                                    string p = Path.Combine(_RecPath, $"{m4s}_tmp");
                                    _mapwriter!.Write($"#EXT-BILI-AUX:0|P|00000|00000000\r\n#EXTINF:1.00,0|00000000\r\n{m4s}\r\n",_videowriter!.Position,new FileInfo(p).Length);
                                    FileStream fs = File.Open(p, FileMode.Open, FileAccess.Read, FileShare.Read);
                                    fs.CopyTo(_videowriter!);
                                    fs.Dispose();
                                    File.Delete(p);
                                }
                                _mapwriter!.Flush();
                            }

                            //补充可能缺少的分片 小于1000是以防404失效以及B站分片突然跳跃
                            if (m4soffset - _PiecesPosition > 1 && m4soffset - _PiecesPosition < 1000)
                            {
                                for (int i = 0; i < m4soffset - _PiecesPosition; i++)
                                {
                                    string m4sname = $"{_PiecesPosition + i}.m4s";
                                    //下载
                                    if (!_ExistsPieces.Contains(m4sname))
                                    {
                                        _ExistsPieces.Add(m4sname);

                                        int retry = 1;
                                        long pos = _videowriter!.Position;
                                    Retry:
                                        try
                                        {
                                            Stream map = Http.GetStream($"{_Host}{m4sname}").Result;
                                            _S_DownloadedBytes += map.Length;
                                            map.CopyTo(_videowriter);
                                            
                                        }
                                        catch (Exception ex)
                                        {
                                            _videowriter.Position = pos;//取消写入
                                            _LogInstance.Log($"Exception at get m4s file '{m4sname}' : {ex.Message},retry {retry} / {MaxRetry}");
                                            Thread.Sleep(100);
                                            if (retry <= MaxRetry)
                                            {
                                                retry++;
                                                goto Retry;
                                            }
                                            else throw;
                                        }
                                        _LogInstance.Log($"Get m4s file '{m4sname}'");
                                        _mapwriter!.Write($"#EXT-BILI-AUX:0|R|00000|00000000\r\n#EXTINF:1.00,0|00000000\r\n{m4sname}\r\n", pos, _videowriter!.Position - pos);
                                    }
                                }
                            }

                            //更新offset
                            _PiecesPosition = m4soffset;
                            //下载
                            if (!_ExistsPieces.Contains(m3u8line))
                            {
                                _ExistsPieces.Add(m3u8line);
                                int retry = 1;
                            Retry:
                                try
                                {
                                    Stream map = Http.GetStream($"{_Host}{m3u8line}").Result;
                                    _S_DownloadedBytes += map.Length;

                                    //复制到tmp检查crc32再复制到视频流
                                    _videowriter_tmp.SetLength(0);
                                    map.CopyTo(_videowriter_tmp);
                                    _videowriter_tmp.Position = 0;
                                }
                                catch (Exception ex)
                                {
                                    _LogInstance.Log($"Exception at get m4s file '{m3u8line}' : {ex.Message},retry {retry} / {MaxRetry}");
                                    Thread.Sleep(100);
                                    if (retry <= MaxRetry)
                                    {
                                        retry++;
                                        goto Retry;
                                    }
                                    else throw;
                                }
                                _LogInstance.Log($"Get m4s file '{m3u8line}'");


                                //CRC32校验
                                int retrycrc = 1;
                                string bilicrc32 = _M3U8Content[listoffset - 1].Split('|').Last();
                                string thiscrc32 = _videowriter_tmp.CRC32().Result;
                                if (!thiscrc32.PadLeft(8, '0').Equals(bilicrc32.PadLeft(8, '0'), StringComparison.CurrentCultureIgnoreCase))
                                {
                                    _LogInstance.Log($"Crc32 failed on '{m3u8line}' : expected {bilicrc32} but get {thiscrc32},retry {retrycrc} / {MaxRetry}");
                                    if (retrycrc <= MaxRetry)
                                    {
                                        retry++;
                                        goto Retry;
                                    }
                                    else
                                    {
                                        string crcf = (Path.Combine(_RecPath, "Crc32FailedList.txt"));
                                        if (!File.Exists(crcf)) File.AppendAllText(crcf, "M4SNAME|VIDEOSTREAM|POSITION|LENGTH|EXPECTEDCRC|ACTURALCRC");
                                        File.AppendAllText(Path.Combine(_RecPath, "Crc32FailedList.txt"),$"{m3u8line}|{_videoindex:D5}.mp4|{_videowriter!.Position}|{_videowriter_tmp.Length}|{bilicrc32}|{thiscrc32}\r\n");
                                        _S_Crc32FailedPieces++;
                                    }
                                }
                                _mapwriter!.Write($"{_M3U8Content[listoffset - 2]}\r\n{_M3U8Content[listoffset - 1]}\r\n{m3u8line}\r\n", _videowriter!.Position, _videowriter_tmp.Length);
                                //复制到video流
                                _videowriter_tmp.Position = 0;
                                _videowriter_tmp.CopyTo(_videowriter!);
                            }
                        }
                        else//跳过
                        {
                            //
                        }
                    }
                    //重新获取m3u8，更新并检查是否在直播
                    _M3U = Http.TryGetResp(_M3U8Url).Result;
                    _InternalCanRec = _M3U.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    Log.LogError($"MESSAGE|Exception at recorder : {ex.Message}", _RoomId);
                    _LogInstance.Log($"Exception at recorder : {ex.Message}");
                    _InternalCanRec = false;//不能录制
                }
                _videowriter?.Flush();
                _mapwriter?.Flush();

                Log.LogInfo($"STAT|{_S_DownloadedBytes - Last_S_DownloadedBytes}|{_ExistsPieces.Count - Last_ExistsPieces}", _RoomId);
                Last_ExistsPieces = _ExistsPieces.Count;
                Last_S_DownloadedBytes = _S_DownloadedBytes;
                //等待0.5s
                Thread.Sleep(500);
            }

            //录制结束
            long endpos = _videowriter != null ? _videowriter.Position : 0;
            _mapwriter?.Write($"#EXT-X-ENDLIST\r\n", endpos, 0);
            _mapwriter?.Flush();
            TimeSpan timeconsume = DateTime.Now - _S_StartTime;
            File.WriteAllText(Path.Combine(_RecPath, "Statistics.log"), Statistics.GetStatisticsString(_RoomId, _M3U8Url, _RecPath, _S_Title, timeconsume, _S_DownloadedBytes, _S_Crc32FailedPieces, _ExistsPieces.Count,_videoindex));

            _ExistsPieces.Clear();
            _videowriter?.Dispose();
            _videowriter_tmp.Dispose();
            _mapwriter?.Close();
            Log.LogInfo($"STATUS_CHANGE|IDLE|Record finished : {_RecPath}", _RoomId);
            _LogInstance.Log($"Record finished : {_RecPath}");
            _IsRec = false;
        }
    }
}
