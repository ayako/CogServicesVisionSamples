using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.Azure.CognitiveServices.Vision.Face;
//using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace CogServicesVisionSamples_202107.Pages
{
    public class MaskRecognitionModel : PageModel
    {
        [BindProperty]
        public IFormFile ImageFile { get; set; }

        public string ImageFileUrl { get; set; }
        public string Result { get; set; }
        public Mask MaskRecognition { get; set; }

        // Setting for using Face API
        private const string faceSubscriptionKey = "YOUR_SUBSCRIPTION_KEY";
        private const string faceEndpoint = "YOUR_SUBSCRIPTION_ENDPOINT"; //"https://japaneast.api.cognitive.microsoft.com/"

        // Setting for FileIO
        private readonly IWebHostEnvironment _host;
        public MaskRecognitionModel(IWebHostEnvironment host)
        {
            _host = host;
        }

        static readonly HttpClient httpClient = new HttpClient();

        public async Task<IActionResult> OnPostAsync()
        {
            // Show image on web page
            var imageFilePath = Path.Combine(_host.WebRootPath, "images\\uploadedImage.jpg");
            using (var fileStream = new FileStream(imageFilePath, FileMode.OpenOrCreate))
            {
                await ImageFile.CopyToAsync(fileStream);
            }
            ImageFileUrl = "/images/uploadedImage.jpg";

            //var faceClient = new FaceClient(new ApiKeyServiceClientCredentials(faceSubscriptionKey))
            //{
            //    Endpoint = faceEndpoint
            //};

            try
            {
                //var faceResult = await faceClient.Face.DetectWithStreamAsync(ImageFile.OpenReadStream(),
                //    returnFaceId: false,
                //    returnFaceLandmarks: false,
                //    returnFaceAttributes: FaceAttributeType.Mask,
                //    detectionModel: "detection_03"
                //    );

                // Call API direct without using Face API Libraries
                var content = new StreamContent(ImageFile.OpenReadStream());
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                var url = faceEndpoint + "face/v1.0/detect?returnFaceAttributes=mask&detectionModel=detection_03";

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add(@"Ocp-Apim-Subscription-Key", faceSubscriptionKey);
                request.Content = content;
                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var faceResponseJson = await response.Content.ReadAsStringAsync();
                    var faceResponse = JsonSerializer.Deserialize<MaskRecognitionResult[]>(faceResponseJson);

                    Result = "結果";
                    MaskRecognition = faceResponse[0].faceAttributes.mask;


                    // Draw rectangle on detected image
                    var detectedImagePath = Path.Combine(_host.WebRootPath, "images\\detectedImage.jpg");
                    var rectangleImagePath = Path.Combine(_host.WebRootPath, "images\\rectangle.png");

                    using (var detectedImage = new Bitmap(imageFilePath))
                    using (var graphics = Graphics.FromImage(detectedImage))
                    {
                        graphics.DrawImage(new Bitmap(rectangleImagePath),
                            faceResponse[0].faceRectangle.left, faceResponse[0].faceRectangle.top,
                            faceResponse[0].faceRectangle.width, faceResponse[0].faceRectangle.height);
                        detectedImage.Save(detectedImagePath, ImageFormat.Jpeg);
                    }

                    ImageFileUrl = "/images/detectedImage.jpg";

                }
                else
                {
                    Result = "判定できませんでした";
                }
            }
            catch (ApplicationException e)
            {
                Result = "エラー: " + e.Message;
            }

            return Page();
        }


        // Custom Class for Mask Recognition
        public class MaskRecognitionResult
        {
            public string faceId { get; set; }
            public FaceRectangle faceRectangle { get; set; }
            public FaceAttributes faceAttributes { get; set; }
        }

        public class FaceRectangle
        {
            public int top { get; set; }
            public int left { get; set; }
            public int width { get; set; }
            public int height { get; set; }
        }

        public class FaceAttributes
        {
            public Mask mask { get; set; }
        }

        public class Mask
        {
            public string type { get; set; }
            public bool noseAndMouthCovered { get; set; }
        }

    }
}
