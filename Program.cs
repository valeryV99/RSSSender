using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Toolkit.Parsers;
using Microsoft.Toolkit.Parsers.Rss;
namespace RSSSender
{
    class Program
    {

        static async Task<string> GetRSS()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://news.google.com/rss?topic=h&hl=en-US&gl=US&ceid=US:en"),
            };
            Console.WriteLine("TEST");
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return await Task.Run(() => response.Content.ReadAsStringAsync());
                // if (body != null)
                // {
                    // Console.WriteLine(body.GetType());
                    // var parser = new RssParser();
                    // parser.Parse(body)
                    // foreach (var element in rss)
                    // {
                        // Console.WriteLine($"Title: {element.Summary}");
                        // Console.WriteLine($"Title: {element.Title}");
                        // Console.WriteLine($"Summary: {element.Summary}");
                    //     return;
                    // }
                // }
                // Console.WriteLine(body);
            }
        }
        
        static void ParseRSS(string rss)
        {
            var parser = new RssParser( );
            var result = parser.Parse(rss);
            foreach (var element in result.Where(a => a.))
            {
                Console.WriteLine($"Title: {element.PublishDate}");
            }
        }

        static void RSSInterval(int num, ElapsedEventHandler method) 
        {
            var aTimer = new System.Timers.Timer();
            aTimer.Interval = num;

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += method;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;
        }
        
        private async static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
            var rss = await GetRSS();
            ParseRSS(rss);
        }
        
        static async Task Main(string[] args)
        {
            RSSInterval(9000);
            Console.ReadLine();
        }
    }
}