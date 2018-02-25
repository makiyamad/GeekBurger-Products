using Microsoft.Extensions.Configuration;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Api.Vision.Sample
{
    class Program
    {
        public static IConfiguration Configuration;
        public static string[] blacklist = new string[]
            { "ingredients", "processed in a facility that handles", "products" , "allergens" , "contains" };
        public const string AndWithSpace = " and ";
        public const string CommaWithSpace = " , ";
        private const string Exit = "exit";

        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var visionServiceClient =
                new VisionServiceClient(Configuration["VisionAPIKey"],
                "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/");

            while (true)
            {
                Console.WriteLine("Detect Ingredients on label images. Type the image name and hit enter");
                string command;

                var templateImage = @"labels\{0}.jpg";
                var imageFilePath = "";

                while (true)
                {
                    command = Console.ReadLine();
                    if (command.Equals(Exit)) break;

                    imageFilePath = string.Format(templateImage, command);

                    if (File.Exists(imageFilePath)) break;
                    Console.WriteLine("File does not exist in folder, try again");
                }

                if (command.Equals(Exit)) break;
                OcrResults results;
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    results = visionServiceClient.RecognizeTextAsync(imageFileStream).Result;
                }
                var lines = results.Regions.SelectMany(region => region.Lines);
                var words = lines.SelectMany(line => line.Words);
                var wordsText = words.Select(word => word.Text.ToUpper());

                var wordsJoint = string.Join(' ', wordsText)
                    .Replace(AndWithSpace, CommaWithSpace, StringComparison.InvariantCultureIgnoreCase);

                foreach (var item in blacklist)
                {
                    wordsJoint = wordsJoint.Replace(item, ",", StringComparison.InvariantCultureIgnoreCase);
                }

                var wordsSplitByComma = wordsJoint.Split(',').ToList();

                Console.WriteLine("Ingredients:");
                wordsSplitByComma
                    .Distinct()
                    .ToList()
                    .ForEach(wordText =>
                {
                    var text = wordText.RemoveSpecialCharacters().Trim();
                    if (!String.IsNullOrWhiteSpace(text))
                        Console.WriteLine(text);
                });

            }
        }
    }
    public static class StringExtensions
    {
        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
