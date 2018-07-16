using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CheckWebSSL
{
    class Program
    {
        static List<Site> websites;
        static void Main(string[] args)
        {
            try {

                Task.WaitAll(CheckSites());
                Console.ReadKey();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
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
        }

        private static async Task CheckSites()
        {
            websites = GetUrls();
            foreach (var website in websites)
              await CheckSite(website);
            
        }

        private static async Task CheckSite(Site web)
        {
            try
            {
                var request = WebRequest.Create(web.URL);
                //   request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";

                Console.WriteLine(request.Method);
                var response = (HttpWebResponse)await Task.Factory
                    .FromAsync<WebResponse>(request.BeginGetResponse,
                                            request.EndGetResponse,
                                            null);
                web.IsEncrypted = true;
            }
            catch(WebException ex)
            {
                web.IsEncrypted = false;
                Console.WriteLine(ex.Status);
            }
          
        }
     
    }

}
