using System;
using System.Diagnostics;
using MailKit.Net.Imap;
using MailKit;
using MimeKit;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using MailKit.Security;
namespace API_Program;

static class SMTP
{
    public static void SendEmail(string subject, string body)
    
    {
        try
            {
                // Konfiguracja klienta SMTP
               using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
               {
                    client.Credentials = new NetworkCredential(GlobalsVariables.email,GlobalsVariables.appPassword);
                    client.EnableSsl = true;
                    

                    // Konfiguracja wiadomości e-mail
                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(GlobalsVariables.email),
                        Subject = subject,
                        Body = body
                    };
                    mailMessage.To.Add("jakub.maka@elis.com");

                    // Wysyłka e-maila
                    client.Send(mailMessage);
                    Console.WriteLine("E-mail wysłany.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas wysyłania e-maila: {ex.Message}");
            }
        
    }
}