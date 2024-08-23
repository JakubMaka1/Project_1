using System;
using System.Diagnostics;
using MailKit.Net.Imap;
using MailKit;
using MimeKit;
using PROJECT_1;

class ProgramRuntime
{
    private Stopwatch stopwatch;

    public ProgramRuntime()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();
    }

    public void StopAndDisplayRuntime()
    {
        stopwatch.Stop();
        TimeSpan runtime = stopwatch.Elapsed;
        Logger.WriteSystemLog($"Program działał przez: {runtime.Hours} godz {runtime.Minutes} min {runtime.Seconds} sek {runtime.Milliseconds} ms");
    }
}
