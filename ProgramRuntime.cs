using System;
using System.Diagnostics;
using MailKit.Net.Imap;
using MailKit;
using MimeKit;
using System.Security.Cryptography.X509Certificates;
namespace API_Program;

class ProgramRuntime
{
    public Stopwatch stopwatch;

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
        SMTP.SendEmail("Program przestał działać", $"Program działał przez: {runtime.Hours} godz {runtime.Minutes} min {runtime.Seconds} sek {runtime.Milliseconds} ms");
        }

    public void ErrorStopAndDisplayRuntime(string error)
    {
        stopwatch.Stop();
        TimeSpan runtime = stopwatch.Elapsed;
        Logger.WriteSystemLog($"Program działał przez: {runtime.Hours} godz {runtime.Minutes} min {runtime.Seconds} sek {runtime.Milliseconds} ms");
        SMTP.SendEmail("Awaria programu", $"{error}, Program działał przez: {runtime.Hours} godz {runtime.Minutes} min {runtime.Seconds} sek {runtime.Milliseconds} ms");
        
        }
}
