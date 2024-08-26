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
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace API_Program;

 class Program
 {
    public static async Task Main(string[] args)
    {
        Logger.ReadLastSystemLog("Program działał przez");
        //wyjscie z programu CRL + C, uznawane jako zamkniecie wiec stoper się zatrzyma
        ProgramRuntime programRuntime = new ProgramRuntime();

        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; // Zapobieganie natychmiastowemu zamknięciu programu
            programRuntime.StopAndDisplayRuntime();
            Environment.Exit(0); // Zamknięcie programu
        };

        //    Console.WriteLine($"Program rozpoczął działanie.");
        //    Logger.WriteSystemLog($"Program rozpoczął działanie.");

        // Pobieranie danych logowania ze zmiennych środowiskowych testowy email z @interia.pl
        string email = Environment.GetEnvironmentVariable("EMAIL_ADDRESS");
        string password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
        if (email == null || password == null)
        {
            Logger.WriteEmailLog("Email or password environment variable is not set.");
        }
        //else
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
                var cancellationTokenSource = new CancellationTokenSource();
                     
                     Console.WriteLine("Monitoring folderu INBOX...");
                     Logger.WriteSystemLog("Monitoring folderu INBOX...");

                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    // Odśwież folder
                    await inbox.CheckAsync(cancellationTokenSource.Token);

                    // Szuka ID nieprzeczytanych wiadomości
                    var uids = await inbox.SearchAsync(SearchQuery.NotSeen); 
                    
                        foreach (var uid in uids)
                        {
                          Logger.CleanOldLogs();//usuwa starsze niz 1 dzien z logów
                          // Wylistowanie nieprzeczytanych wiadomości
                          var message = await inbox.GetMessageAsync(uid);
                          
                          Console.WriteLine($"Odebrano wiadomość: {message.Subject}");
                          Logger.WriteEmailLog($"Odebrano wiadomość: {message.Subject}");
                          

                         if (message.Subject.Contains("disable", StringComparison.OrdinalIgnoreCase))
                            {
                                string jsonData = "{ \"status\": \"disable\" }";

                                // Wykonanie zapytania PUT
                                
                                await SendPutRequest(jsonData);
                                // Wykonanie zapytania GET
                                await SendGETRequest();
                                
                            }
                         if (message.Subject.Contains("enable", StringComparison.OrdinalIgnoreCase))
                            {
                                string jsonData = "{ \"status\": \"enable\" }";

                                // Wykonanie zapytania PUT
                                await SendPutRequest(jsonData);
                                // Wykonanie zapytania GET
                                await SendGETRequest();
                            } 

                         // Po przetworzeniu wiadomości możesz ją oznaczyć jako przeczytaną
                         await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
                                                  
                        }
                     // Sprawdzenie co 10 sekund
                     await Task.Delay(10000, cancellationTokenSource.Token);
                                        
                }

                 // Rozłącz się z serwerem IMAP
                 await client.DisconnectAsync(true);
                 
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operacja anulowana.");
                Logger.WriteEmailLog("Operacja anulowana.");
                programRuntime.StopAndDisplayRuntime();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                Logger.WriteEmailLog($"Wystąpił błąd: {ex.Message}");
                programRuntime.StopAndDisplayRuntime();
            }
        }    
        
        programRuntime.StopAndDisplayRuntime();
       
    }

    private static async Task SendPutRequest(string jsonData)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync("http://10.10.10.4/api/v2/cmdb/firewall/policy/3/?access_token=gtjmQkf4s1kQwQ9yd3nhmfxx39ykb9", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Operacja aktualizacji statusu udana.");
                    Logger.WriteEmailLog("Operacja aktualizacji statusu udana.");
                }
                else
                {
                    Console.WriteLine($"Wystąpił błąd: {response.StatusCode}");
                    Logger.WriteEmailLog($"Wystąpił błąd: {response.StatusCode}");
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
                                Logger.WriteEmailLog($"Nazwa policy: {result["name"]}");
                                Logger.WriteEmailLog($"Aktualny status: {result["status"]} ");                                
                            }
                        }
            }
        }
 }
