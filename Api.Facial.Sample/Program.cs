using Microsoft.Extensions.Configuration;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Facial.Sample
{
    class Program
    {
        public static IConfiguration Configuration;
        public static FaceServiceClient faceServiceClient;
        public static Guid FaceListId;
        private const string Exit = "exit";
        static void Main(string[] args)
        {
            var templateImage = @"faces\{0}.jpg";
            var sourceImage = "";

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            FaceListId = Guid.Parse(Configuration["FaceListId"]);

            faceServiceClient = new FaceServiceClient(Configuration["FaceAPIKey"], "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/");

            Console.WriteLine("Face Detection console. Please inform file name to check and hit enter. To exit, type exit");

            var command = "";
            while (true)
            {
                try
                {
                    while (true)
                    {
                        command = Console.ReadLine();
                        if (command.Equals(Exit)) break;

                        sourceImage = string.Format(templateImage, command);

                        if (File.Exists(sourceImage)) break;
                        Console.WriteLine("File does not exist in folder, try again");
                    }

                    if (command.Equals(Exit)) break;

                    var containsAnyFaceOnList = UpsertFaceListAndCheckIfContainsFaceAsync().Result;

                    var face = DetectFaceAsync(sourceImage).Result;
                    if (face != null)
                    {
                        Guid? persistedId = null;
                        if (containsAnyFaceOnList)
                            persistedId = FindSimilarAsync(face.FaceId, FaceListId).Result;

                        if (persistedId == null)
                        {
                            persistedId = AddFaceAsync(FaceListId, sourceImage).Result;
                            Console.WriteLine($"New User with FaceId {persistedId}");
                        }
                        else
                            Console.WriteLine($"Face Exists with Face {persistedId}");
                    }
                    else
                    {
                        Console.WriteLine("Not a face!");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Probably Rate Limit for API was reached, please try again later");
                }
            }
        }

        private static async Task<bool> UpsertFaceListAndCheckIfContainsFaceAsync()
        {
            var faceListId = FaceListId.ToString();
            var faceLists = await faceServiceClient.ListFaceListsAsync();
            var faceList = faceLists.FirstOrDefault(_ => _.FaceListId == FaceListId.ToString());

            if (faceList == null)
            {
                await faceServiceClient.CreateFaceListAsync(faceListId, "GeekBurgerFaces", null);
                return false;
            }

            var faceListJustCreated = await faceServiceClient.GetFaceListAsync(faceListId);

            return faceListJustCreated.PersistedFaces.Any();
        }

        private static async Task<Guid?> FindSimilarAsync(Guid faceId, Guid faceListId)
        {
            var similarFaces = await faceServiceClient.FindSimilarAsync(faceId, faceListId.ToString());

            var similarFace = similarFaces.FirstOrDefault(_ => _.Confidence > 0.5);

            return similarFace?.PersistedFaceId;
        }

        private static async Task<Face> DetectFaceAsync(string imageFilePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream);
                    return faces.FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<Guid?> AddFaceAsync(Guid faceListId, string imageFilePath)
        {
            try
            {
                AddPersistedFaceResult faceResult;
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    faceResult = await faceServiceClient.AddFaceToFaceListAsync(faceListId.ToString(), imageFileStream);
                    return faceResult.PersistedFaceId;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Face not included in Face List!");
                return null;
            }
        }
    }
}
