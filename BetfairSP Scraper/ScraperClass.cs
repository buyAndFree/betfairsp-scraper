using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace BetfairSP_Scraper
{
    class ScraperClass
    {
        /// <summary>
        /// 
        /// </summary>
        static void Main()
        {
            var cancelSource = new CancellationTokenSource();
            var hcHandler = new HttpClientHandler() 
            { 
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate 
            };
            var httpClient = new HttpClient(hcHandler);
            var bfAgent = new BetfairAgent(httpClient);

            Console.WriteLine("Betfair SP File Scraper");
            Task downloadTask = null;
            while (true)
            {
                string input = Console.ReadLine();
                if (input == "start")
                {
                    if ((downloadTask != null) && (downloadTask.IsCompleted == false))
                    {
                        Console.WriteLine("Download Task is already running");
                        continue;
                    }
                    Console.Write("Country (uk, ire, aus, usa, rsa): ");
                    string country = Console.ReadLine().ToLower();
                    if (!(country == "uk" || country == "ire" || country == "aus" || country == "usa" || country == "rsa"))
                    {
                        Console.WriteLine("Invalid Country");
                        continue;
                    }
                    Console.Write("Bet type (win, place): ");
                    string betType = Console.ReadLine().ToLower();
                    if (!(betType == "win" || betType == "place"))
                    {
                        Console.WriteLine("Invalid Bettype");
                        continue;
                    }
                    Console.Write("Greyhound Only? (yes, no): ");
                    string greyhound = Console.ReadLine().ToLower();
                    if (!(greyhound == "yes" || greyhound == "no"))
                    {
                        Console.WriteLine("Invalid Selection");
                        continue;
                    }
                    Console.Write("Start Date (dd/mm/yyyy):");
                    string startDateStr = Console.ReadLine();
                    Console.Write("End Date (dd/mm/yyyy):");
                    string endDateStr = Console.ReadLine();
                    DateTime startDate;
                    DateTime endDate;
                    var enUS = new CultureInfo("en-US");
                    if (DateTime.TryParseExact(startDateStr, "dd/MM/yyyy", enUS, DateTimeStyles.None, out startDate) && DateTime.TryParseExact(endDateStr, "dd/MM/yyyy", enUS, DateTimeStyles.None, out endDate))
                    {
                        if (startDate <= endDate)
                        {
                            downloadTask = bfAgent.DownloadBetfairSPFiles(country, betType, greyhound, startDate, endDate, cancelSource.Token);
                        }
                        else
                        {
                            Console.WriteLine("Error: End Date is before Starting Date!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unable to parse dates");
                        continue;
                    }
                }
                else if (input == "exit")
                {
                    cancelSource.Cancel();
                    Console.WriteLine("Exiting...");
                    break;
                }
                else if (input == "stoptasks")
                {
                    Console.WriteLine("Stopping downloads...");
                    cancelSource.Cancel();
                    while (true)
                    {
                        Thread.Sleep(10);
                        if (downloadTask.IsCanceled || downloadTask.IsCompleted || downloadTask.IsFaulted)
                        {
                            #region cancel the token source
                            try
                            {
                                cancelSource.Cancel();   // If there's no timeout, the tokensource won't have been asked to cancel
                            }
                            catch (ObjectDisposedException)
                            {
                                // Already disposed
                            }
                            catch (AggregateException)
                            {
                                // AggregateException 
                            }
                            break;
                            #endregion
                        }
                    }
                    cancelSource = new CancellationTokenSource();
                    //downloadTask = null;
                    Console.WriteLine("Downloading stopped.");
                }
                else
                {
                    Console.WriteLine("Invalid Command - Valid Commands are 'start', 'stoptasks', 'exit'");
                }
            }
        }
    }
}
