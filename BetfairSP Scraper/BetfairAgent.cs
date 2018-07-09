using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BetfairSP_Scraper
{
    class BetfairAgent
    {
        private const string betfairURI = "https://promo.betfair.com/betfairsp/prices/";
        private static string rootDownloadDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private HttpClient httpClient;
        public BetfairAgent(HttpClient client)
        {
            httpClient = client;
        }
        /// <summary>
        /// This method asynchronously downloads betfair startprice files and writes them to a csv file based on the input parameters given.
        /// File types are determined by location, bet type, date, and type of race (greyhound/horses).
        /// </summary>
        /// <param name="location">Selected location (United Kingdom = uk, Ireland = ire, South Africa = rsa, Australia = aus, USA = usa).</param>
        /// <param name="bettype">Selected betting type (win or place).</param>
        /// <param name="greyhound">Download greyhound data only (location becomes irrelevant for this case).</param>
        /// <param name="startDate">Starting date for selected downloads.</param>
        /// <param name="endDate">Ending date for the selected downloads.</param>
        /// <param name="token">Cancellation token, required if called by user to stop downloads.</param>
        /// <returns></returns>
        public async Task DownloadBetfairSPFiles(string location, string bettype, string greyhound, DateTime startDate, DateTime endDate, CancellationToken token)
        {
            if ((location == "uk" || location == "ire" || location == "aus" || location == "rsa" || location == "usa") 
                && (bettype == "win" || bettype == "place") 
                && (greyhound == "yes" || greyhound == "no") 
                && (startDate <= endDate))
            {
                // The following strings appended to the root URL for downloading are dependent on betfair file naming conventions.
                var baseLocalFileName = "dwbf";
                if (greyhound == "yes")
                {
                    baseLocalFileName = baseLocalFileName + "greyhound";
                }
                else
                {
                    baseLocalFileName = baseLocalFileName + "prices" + location;
                }
                baseLocalFileName = baseLocalFileName + bettype;
                string url = betfairURI + baseLocalFileName;
                while (startDate <= endDate)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Downloading Task Cancelled!");
                        return;
                    }
                    using (HttpResponseMessage response = await this.httpClient.GetAsync(url + (startDate.ToString("ddMMyyyy") + ".csv"), HttpCompletionOption.ResponseHeadersRead))
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        string fileToWriteTo = rootDownloadDirectory + (baseLocalFileName + startDate.ToString("ddMMyyyy") + ".csv");
                        using (Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
                        }
                    }
                    Console.WriteLine("<Downloaded " + startDate.ToString() + ">");
                    startDate = startDate.AddDays(1);
                }
                Console.WriteLine("Downloading Complete!");
            }
            return;
        }
    }
}
