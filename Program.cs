using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Toolkit.Parsers.Rss;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Net.Mail;

namespace RSSSender
{
    class Сriteria
    {
        public int PublishDate { get; set; }
    }
    class NewsItem 
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }
    }
    class Program
    {
        static string[] emails;
        static Сriteria criteria = new ();
        static ObservableCollection<NewsItem> news = new();
        
        static void News_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    Console.WriteLine("Reset");
                    break;
            }
        }
        static async Task<string> GetRSS()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://news.google.com/rss?topic=h&hl=en-US&gl=US&ceid=US:en"),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return await Task.Run(() => response.Content.ReadAsStringAsync());
            }
        }
        
        static void ParseRSS(string rss)
        {
            var parser = new RssParser( );
            var result = parser.Parse(rss);
            foreach (var element in result.Where(item => item.PublishDate.Day == criteria.PublishDate))
            {
                Console.WriteLine($"PublishDate: {element.PublishDate.Day}");
                news.Add(new NewsItem
                {
                    Title = element.Title,
                    PublishDate = element.PublishDate,
                    Content = element.Content,
                });
            }
        }

        static void RSSSender()
        {
            foreach (var email in emails)
            {
                MailAddress fromMailAddress = new MailAddress("green_arrowalera@mail.ru", "ValeryMailRu");
                MailAddress toMailAddress = new MailAddress(email, email);
                using (MailMessage mailMessage = new MailMessage(fromMailAddress, toMailAddress))
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    mailMessage.Subject = "News";
                    foreach (var newsItem in news)
                    {
                        mailMessage.Body += newsItem.Content;   
                    }
                    smtpClient.Host = "smtp.mail.ru";
                    smtpClient.Port = 465;
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(fromMailAddress.Address, "");
                    smtpClient.Timeout = 100;
                    try
                    {
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                            ex.ToString());
                    }
                }   
            }
            news.Clear();
            Console.WriteLine("sent");
        }
        static void Interval(int num, ElapsedEventHandler method) 
        {
            var aTimer = new System.Timers.Timer();
            aTimer.Interval = num;
            
            aTimer.Elapsed += method;
            
            aTimer.AutoReset = true;
            
            aTimer.Enabled = true;
        }
        
        private async static void OnGetRSSEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
            var rss = await GetRSS();
            ParseRSS(rss);
        }

        private async static void OnRSSSenderEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            RSSSender();
        }
        static void Main(string[] args)
        {
            emails =  new[] {"test@test.com", "test2@test.com"};
            criteria.PublishDate = 6;
            news.CollectionChanged += News_CollectionChanged;
            Interval(9000, OnGetRSSEvent);
            Interval(10000, OnRSSSenderEvent);
            Console.ReadLine();
        }
    }
}