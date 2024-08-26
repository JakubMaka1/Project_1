using System;
using System.Diagnostics;
using MailKit.Net.Imap;
using MailKit;
using MimeKit;
using System.Net;
using System.Net.Mail;
using System.Reflection;
namespace API_Program;

static class SMTP
{
    public static void SendEmail(string subject, string body)
    {
        //jak w program.cs by wiedziało ze ma brac zmeinne ze środowiskowych
        string email = Environment.GetEnvironmentVariable("EMAIL_ADDRESS");
        string password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

        try
            {
                
                // Konfiguracja klienta SMTP
               SmtpClient client = new SmtpClient("poczta.interia.pl", 587)
                {
                    Credentials = new NetworkCredential(email, password),
                    EnableSsl = true
                };

                // Konfiguracja wiadomości e-mail
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(email),
                    Subject = subject,
                    Body = body
                };
                mailMessage.To.Add("jakub.maka@elis.com");

                // Wysyłka e-maila
                client.Send(mailMessage);
                Console.WriteLine("E-mail wysłany.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas wysyłania e-maila: {ex.Message}");
            }
        
    }
}