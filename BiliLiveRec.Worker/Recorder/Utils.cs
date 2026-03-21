using BiliLiveRec.Worker.Network;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Nodes;

namespace BiliLiveRec.Worker.Recorder
{
    internal class Utils
    {
        public static string UGetUrl0(JsonNode json)
        {
            string host = String.Empty;
            string turl = String.Empty;
            if (json["data"]!["playurl_info"] == null) throw new Exception($"Error : No stream returned.");
            JsonArray jsonArray = json["data"]!["playurl_info"]!["playurl"]!["stream"]!.AsArray();
            foreach (var v in jsonArray)
            {
                if ((string)v!["protocol_name"]! == "http_hls")
                {
                    foreach (var v2 in v["format"]!.AsArray())
                    {
                        if ((string)v2!["format_name"]! == "fmp4")
                        {
                            foreach (var hr in v2["codec"]![0]!["url_info"]!.AsArray())
                            {
                                string hhost = (string)hr!["host"]!;
                                if (!hhost.Contains("gotcha") && hhost.Contains(".bilivideo.com"))
                                {
                                    host = hhost;
                                    break;
                                }
                                else
                                {
                                    host = "https://cn-sxxa-cm-01-02.bilivideo.com";//default
                                }
                            }
                            turl = (string)v2!["codec"]![0]!["base_url"]!;
                        }
                        else if ((string)v2!["format_name"]! == "ts")
                        {
                            if (host == String.Empty || turl == String.Empty)
                            {
                                foreach (var hr in v2["codec"]![0]!["url_info"]!.AsArray())
                                {
                                    string hhost = (string)hr!["host"]!;
                                    if (!hhost.Contains("gotcha") && hhost.Contains(".bilivideo.com"))
                                    {
                                        host = hhost;
                                        break;
                                    }
                                    else
                                    {
                                        host = "https://cn-sxxa-cm-01-02.bilivideo.com";//default
                                    }

                                }
                                turl = (string)v2!["codec"]![0]!["base_url"]!;

                                turl = turl.Replace(".m3u8", "/index.m3u8");
                            }
                        }
                        //

                    }
                }
            }
            string burl = turl;
            foreach (string s in ReplaceOptions.Option)
            {
                burl = burl.Replace(s, "");
            }
            return $"{host}{burl.Replace("?", "")}";
        }
    }
}
