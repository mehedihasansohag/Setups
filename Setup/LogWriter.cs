// Decompiled with JetBrains decompiler
// Type: Setup.LogWriter
// Assembly: Setup, Version=3.304.2.0, Culture=neutral, PublicKeyToken=null
// MVID: 7B0909A6-AB6D-4CC2-A916-03083FF75494
// Assembly location: C:\Program Files\Weihong\NcStudio\Bin\PackUp\Setup.exe

using Packup.Library;
using System;

namespace Setup
{
    internal static class LogWriter
    {
        static LogWriter() => Log.LogFileName = "Install.log";

        internal static event LogWriter.WriteEventHandler NotifyProgressChanged;

        internal static void UpdatePrompt(string text)
        {
            Log.Write(text);
            LogWriter.WriteEventHandler notifyProgressChanged = LogWriter.NotifyProgressChanged;
            if (notifyProgressChanged == null)
                return;
            notifyProgressChanged((object)null, new LogEventArgs()
            {
                Message = text
            });
        }

        internal static void Write(string text, bool notifyProgressChanged = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(text))
                    return;
                Log.Write(text);
                if (!notifyProgressChanged)
                    return;
                LogWriter.WriteEventHandler notifyProgressChanged1 = LogWriter.NotifyProgressChanged;
                if (notifyProgressChanged1 == null)
                    return;
                notifyProgressChanged1((object)null, new LogEventArgs()
                {
                    Message = text
                });
            }
            catch (Exception ex)
            {
            }
        }

        internal delegate void WriteEventHandler(object sender, LogEventArgs e);
    }
}
