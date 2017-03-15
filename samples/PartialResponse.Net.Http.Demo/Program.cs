using Microsoft.Owin.Hosting;
using System;

namespace PartialResponse.Net.Http.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9000/"; 

            using (WebApp.Start<Startup>(url: baseAddress)) 
            {
                Console.ReadLine();
            } 
        }
    }
}
