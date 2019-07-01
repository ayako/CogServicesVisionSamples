using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CogServicesVisionSamples_201906.Pages
{
    public class FaceModel : PageModel
    {
        // Input from web page
        [BindProperty]
        public IFormFile ImageFile { get; set; }

        // Output to web page
        public string ImageFileUrl { get; set; }
        public string Result { get; set; }
        public Emotion Emotion { get; set; }

        // Setting for using Face API 
        private const string faceSubscriptionKey = "YOUR_SUBSCRIPTION_KEY";
        private const string faceEndpoint = "YOUR_SUBSCRIPTION_ENDPOINT"; //"https://japaneast.api.cognitive.microsoft.com"

        // Setting for FileIO
        private readonly IHostingEnvironment _host;
        public FaceModel(IHostingEnvironment host)
        {
            _host = host;
        }

        //public void OnGet()
        //{

        //}

        public async Task<IActionResult> OnPostAsync()
        {
            // Show image on web page
            var imageFilePath = Path.Combine(_host.WebRootPath, "images\\uploadedImage.png");
            using (var fileStream = new FileStream(imageFilePath, FileMode.OpenOrCreate))
            {
                await ImageFile.CopyToAsync(fileStream);
            }
            ImageFileUrl = "/images/uploadedImage.png";

            // Post image to Custom Vision, get resut and show on web page
            var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(faceSubscriptionKey))
            {
                Endpoint = faceEndpoint
            };
            var faceAttibute = new FaceAttributeType[] { FaceAttributeType.Emotion } ;

            try
            {
                var faceResult = await faceClient.Face.DetectWithStreamAsync(
                    ImageFile.OpenReadStream(),false,false, faceAttibute);
                if (faceResult.Count > 0)
                {
                    Result = "結果:";
                    Emotion = faceResult[0].FaceAttributes.Emotion;
                }
                else
                {
                    Result = "判定できませんでした";
                }

                // Draw rectangle on detected image
                var detectedImagePath = Path.Combine(_host.WebRootPath, "images\\detectedImage.png");
                var rectangleImagePath = Path.Combine(_host.WebRootPath, "images\\rectangle.png");

                using (var detectedImage = new Bitmap(imageFilePath))
                using (var graphics = Graphics.FromImage(detectedImage))
                {
                    graphics.DrawImage(new Bitmap(rectangleImagePath),
                        faceResult[0].FaceRectangle.Left, faceResult[0].FaceRectangle.Top,
                        faceResult[0].FaceRectangle.Width, faceResult[0].FaceRectangle.Height);
                    detectedImage.Save(detectedImagePath, ImageFormat.Png);
                }

                ImageFileUrl = "/images/detectedImage.png";


            }
            catch (APIErrorException e)
            {
                Result = "エラー: " + e.Message;
            }

            return Page();
        }

    }
}