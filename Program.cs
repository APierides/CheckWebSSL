using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CheckWebSSL
{
    class Program
    {
        static void Main(string[] args)
        {
            try { 
            CheckSite().Wait();
            Console.ReadKey();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task CheckSite()
        {  
            var request = WebRequest.Create("https://www.stackoverflow.com");
            var response = (HttpWebResponse)await Task.Factory
                .FromAsync<WebResponse>(request.BeginGetResponse,
                                        request.EndGetResponse,
                                        null);
            Debug.Assert(response.StatusCode == HttpStatusCode.OK);            
        }
    }
}
