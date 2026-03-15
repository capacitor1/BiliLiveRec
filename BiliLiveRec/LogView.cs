using System;
using System.Collections.Generic;
using System.Text;

namespace BiliLiveRec
{
    internal static class LogView
    {
        private static readonly Lock _lockObj = new Lock();
        public static void Info(this RichTextBox r, string msg,string origin)
        {
            lock (_lockObj)
            {
                r.SelectionColor = default;
                r.AppendText($"[{DateTime.Now}] [{origin}] {msg}");
                r.AppendText(Environment.NewLine);
            }
        }
        public static void InfoGreen(this RichTextBox r, string msg, string origin)
        {
            lock (_lockObj)
            {
                r.SelectionColor = Color.Green;
                r.AppendText($"[{DateTime.Now}] [{origin}] {msg}");
                r.AppendText(Environment.NewLine);
            }
        }
        public static void Error(this RichTextBox r, string msg, string origin)
        {
            lock (_lockObj)
            {
                r.SelectionColor = Color.White;

                r.SelectionBackColor = Color.Red;
                r.AppendText($"[{DateTime.Now}] [{origin}] {msg}");
                r.AppendText(Environment.NewLine);
                r.SelectionBackColor = default;
            }
        }
        public static void Warn(this RichTextBox r, string msg, string origin)
        {
            lock (_lockObj)
            {
                r.SelectionColor = Color.White;

                r.SelectionBackColor = Color.Orange;
                r.AppendText($"[{DateTime.Now}] [{origin}] {msg}");
                r.AppendText(Environment.NewLine);
                r.SelectionBackColor = default;
            }
        }
    }
}
