using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

class Program
{
    static async Task Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Console.WriteLine("Program rozpoczął działanie.");

        await IMAP();

        stopwatch.Stop();

        Console.WriteLine("Program zakończył działanie.");
        TimeSpan elapsedTime = stopwatch.Elapsed;
        Console.WriteLine($"Czas działania programu: {elapsedTime.Hours} godzin, {elapsedTime.Minutes} minut, {elapsedTime.Seconds} sekund, {elapsedTime.Milliseconds} milisekund");
    }

    static async Task IMAP()
    {
        // Pobieranie danych logowania ze zmiennych środowiskowych
        string email = Environment.GetEnvironmentVariable("EMAIL_ADDRESS");
        string password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
        if (email == null || password == null)
        {
            Console.WriteLine("Email or password environment variable is not set.");
        }
        else
        //{
        //    Console.WriteLine($"EMAIL_ADDRESS: {email}");
        //    Console.WriteLine($"EMAIL_PASSWORD: {password}");
        //}
        using (var client = new ImapClient())
        {
            try
            {
                // Połącz się z serwerem IMAP
                await client.ConnectAsync("poczta.interia.pl", 993, true);
                // Zaloguj się na konto e-mail
                await client.AuthenticateAsync(email, password);
                // Otwórz folder INBOX
                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadWrite);
                // Zapamiętaj początkową liczbę wiadomości
                int previousMessageCount = inbox.Count;
                var cancellationTokenSource = new CancellationTokenSource();
                // Pobierz nową wiadomość
                    var newest_message = await inbox.GetMessageAsync(inbox.Count - 1);
                     Console.WriteLine($"{DateTime.Today} Rozpoczęto program");
                     Console.WriteLine($"Najnowsza wiadomość to: {newest_message.Subject}");
                     Console.WriteLine("");
                     Console.WriteLine("Monitoring folderu INBOX...");

                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    // Odśwież folder
                    await inbox.CheckAsync(cancellationTokenSource.Token);

                    // Szuka ID nieprzeczytanych wiadomości
                    var uids = await inbox.SearchAsync(SearchQuery.NotSeen); 
                    
                    // Sprawdź, czy liczba wiadomości wzrosła
                    if (inbox.Count > previousMessageCount)
                    {
                        foreach (var uid in uids)
                        {
                          // Wylistowanie nieprzeczytanych wiadomości
                          var message = await inbox.GetMessageAsync(uid);
                          Console.WriteLine($"Odebrano wiadomość: {message.Subject}");
                         
                          if (message.Subject.Contains("test", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine($"Odebrano wiadomość: {message.Date} {uid}");  
                            }          
                            // Po przetworzeniu wiadomości możesz ją oznaczyć jako przeczytaną
                         await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
                         previousMessageCount = inbox.Count;
                        }
                    // Sprawdzenie co 10 sekund
                    await Task.Delay(10000, cancellationTokenSource.Token);
                    }
                }

                 // Rozłącz się z serwerem IMAP
                 await client.DisconnectAsync(true);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operacja anulowana.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
            }
            //await Task.Delay(5000); // Symulacja pracy trwającej 20 sekund
        }
    }
}


                    
