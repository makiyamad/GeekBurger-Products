using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Api.Consumer.Sample
{
    static class Program
    {
        private static IConfiguration _configuration;
        private static string Uri;
        private static string Subscription;

        static void Main()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            Subscription = _configuration.GetSection("product")["subscription"];
            Uri = _configuration.GetSection("product")["uri"];

            while (true) { 
                MakeRequest();
                var exit = Console.ReadLine();
                if (exit == "exit") break;
            }
        }

        static async void MakeRequest()
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key",
                Subscription);

            HttpResponseMessage response;

            using (var content = new ByteArrayContent(ProductBuild.NewProduct()))
            {
                content.Headers.ContentType = 
                    new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(Uri, content);
            }

            if (response.IsSuccessStatusCode)
            {
                var responseText = await response.Content.ReadAsStringAsync();
                Console.WriteLine("New Product Added");
                Console.WriteLine(responseText);
            }
            else
            {
                Console.WriteLine($"error: {response.StatusCode}");
            }
        }
    }
}