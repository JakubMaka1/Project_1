using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Org.BouncyCastle.Tls;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main(string[] args)
    {
          Console.WriteLine($"Program rozpoczął działanie. {DateTime.Now}");
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
                         
                          if (message.Subject.Contains("STOP", StringComparison.OrdinalIgnoreCase))
                            {
                                string jsonData = "{ \"status\": \"disable\" }";

                                Console.WriteLine($"Odebrano wiadomość: {message.Date} {uid}");  
                                // Wykonanie zapytania PUT
                                await SendPutRequest(jsonData);
                                // Wykonanie zapytania GET
                                await SendGETRequest();
                            }
                            
                            
                            if (message.Subject.Contains("START", StringComparison.OrdinalIgnoreCase))
                            {
                                string jsonData = "{ \"status\": \"enable\" }";

                                Console.WriteLine($"Odebrano wiadomość: {message.Date} {uid}");  
                                // Wykonanie zapytania PUT
                                await SendPutRequest(jsonData);
                                // Wykonanie zapytania GET
                                await SendGETRequest();
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
           
        }    
    }

    private static async Task SendPutRequest(string jsonData)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync("http://10.10.10.4/api/v2/cmdb/firewall/policy/3/?access_token=gtjmQkf4s1kQwQ9yd3nhmfxx39ykb9", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Dane zostały pomyślnie zaktualizowane na serwerze.");
                }
                else
                {
                    Console.WriteLine($"Wystąpił błąd: {response.StatusCode}");
                }
            }
        }
    
    private static async Task SendGETRequest()
        {
            using (var httpClient = new HttpClient())
            {
                
                HttpResponseMessage response = await httpClient.GetAsync("http://10.10.10.4/api/v2/cmdb/firewall/policy/3?access_token=gtjmQkf4s1kQwQ9yd3nhmfxx39ykb9");
                string json = await response.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(json);
                     foreach (var result in jsonObject["results"])
                        {
                            if ((int)result["policyid"] == 3)
                            {
                                // Wyświetlanie statusu
                                Console.WriteLine($"Nazwa policy: {result["name"]}");
                                Console.WriteLine($"Status dla policy z id 3: {result["status"]}");
                                Console.WriteLine($"Data zmiany statusu: {DateTime.Now}");
                            }
                        }
            }
        }
}


                    
