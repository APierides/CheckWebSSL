using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CheckWebSSL
{
    class Program
    {
        static List<Site> websites;
        static int ct = 1;
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;//| SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            try
            {
               Console.WriteLine(ServicePointManager.SecurityProtocol);
               CheckSites().Wait(300000);
               WriteResults();
              Console.ReadKey(); 
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }

     

        private static void WriteResults()
        {
            using (var writer = new StreamWriter(@"D:\Clients\Sensis\SensisURLSResults.CSV"))
            {
                websites.ForEach(web =>
                {
                    writer.WriteLine(web.URL + "," + web.Message + "," + web.InteriorMessage+',' + web.IsEncrypted + "," + web.Status);
                });
              
            }
        }

        private static List<Site> GetUrls()
        {
            var sites = new List<Site>();
            using (var reader = new StreamReader(File.OpenRead(@"D:\Clients\Sensis\Sensis URLSv2.CSV")))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    sites.Add(new Site() { URL = reader.ReadLine().Split(',')[5] });
                }
            }
            return sites;
        }
         private class Site
        {
            public string URL { get; set; }
            public bool IsEncrypted { get; set; }
            public string Message { get; set; }
            public string Status { get; set; }
            public string InteriorMessage { get; set; }

        }

        private static async Task CheckSites()
        {
            websites = GetUrls();
            //websites = new List<Site>()
            //{
            //    new Site()
            //    {
            //        URL = "https://www.advancedheavyvehicles.com.au"
            //    },
            //          new Site()
            //    {
            //        URL = "https://inbox.google.com"
            //    },
            //          new Site()
            //    {
            //        URL = "https://stackoverflow.com"
            //    },
            //          new Site()
            //    {
            //        URL = "https://www.theage.com.au/"
            //    },new Site()
            //    {
            //        URL = "https://tipping.afl.com.au"
            //    },
            //          new Site()
            //    {
            //        URL = "https://www.advancedheavyvehicles.com.au"
            //    }
            //};

            Task.WhenAll(websites.Select(x =>  CheckSite(x)));
         
             
        }

        private static async Task CheckSite(Site web)
        {
            try
            {
                Console.Clear();
                Console.WriteLine(ct);
                ct++;
                if (web.URL == "https:")
                {
                    web.IsEncrypted = false;
                    web.Status = "No Site";
                    return;
                }
                // File.WriteAllText("sample.txt", new HttpClient().GetStringAsync("https://www.howsmyssl.com").Result);
                var request = (HttpWebRequest)WebRequest.Create(web.URL);
                request.Method = "GET";
                request.Accept = "text/html";
                request.UseDefaultCredentials = true;
                request.CookieContainer = new CookieContainer();
                request.Accept = @"text/html, application/xhtml+xml, */*";
                request.Referer = @"http://www.yellow.com/";
                request.Headers.Add("Accept-Language", "en-GB");
                request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
                //request.Host = @"www.yellow.com/";

              //  request.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36");
                var response = (HttpWebResponse)await Task.Factory
                .FromAsync<WebResponse>(request.BeginGetResponse,
                                        request.EndGetResponse,
                                        null);
                
                web.IsEncrypted = response.StatusCode.ToString() == "OK";
                web.Status = response.StatusCode.ToString();
                web.Message = response.StatusDescription;
            }
            catch(WebException ex)
            {
                web.IsEncrypted = false;
               
             
             //   Console.WriteLine(web.URL + " " + ex.Message);
                web.Status = ex.Status.ToString();
                web.Message = ex.Message;
                web.InteriorMessage = ex.InnerException != null ? ex.Message : "";

            }
          
        }
     
    }

}
