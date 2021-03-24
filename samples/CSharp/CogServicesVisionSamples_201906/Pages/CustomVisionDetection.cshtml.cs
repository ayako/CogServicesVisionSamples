using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;

namespace CogServicesVisionSamples_201906.Pages
{
    public class CustomVisionDetectionModel : PageModel
    {
        // Input from web page
        [BindProperty]
        public IFormFile ImageFile { get; set; }

        // Output to web page
        public string ImageFileUrl { get; set; }
        public string Result { get; set; }
        public IList<PredictionModel> Predictions { get; set; }

        // Setting for using Custom Vision 
        private const string cvPredictionKey = "YOUR_CUSTOMVISION_PREDICTION_KEY";
        private const string cvEndpoint = "https://YOUR_LOCATION.api.cognitive.microsoft.com";
        private const string cvProjectId = "YOUR_CUSTOMVISION_PROJECTID";
        private const string cvPublishName = "YOUR_CUSTOMVISION_PROJECT_PUBLISHNAME";//"Iteration1"

        // Setting for FileIO
        private readonly IHostingEnvironment _host;
        public CustomVisionDetectionModel(IHostingEnvironment host)
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
            var cvClient = new CustomVisionPredictionClient()
            {
                Endpoint = cvEndpoint,
                ApiKey = cvPredictionKey
            };

            try
            {
                var cvResult = await cvClient.DetectImageAsync(
                    Guid.Parse(cvProjectId), cvPublishName, ImageFile.OpenReadStream());
                if (cvResult.Predictions.Count > 0)
                {
                    Result = "結果:";
                    Predictions = cvResult.Predictions;

                    // Draw rectangle on detected image
                    var detectedImagePath = Path.Combine(_host.WebRootPath, "images\\detectedImage.png");
                    var rectangleImagePath = Path.Combine(_host.WebRootPath, "images\\rectangle.png");

                    using (var detectedImage = new Bitmap(imageFilePath))
                    using (var graphics = Graphics.FromImage(detectedImage))
                    {
                        graphics.DrawImage(new Bitmap(rectangleImagePath), 
                            (int)(cvResult.Predictions[0].BoundingBox.Left * detectedImage.Width),
                            (int)(cvResult.Predictions[0].BoundingBox.Top * detectedImage.Height),
                            (int)(cvResult.Predictions[0].BoundingBox.Width * detectedImage.Width),
                            (int)(cvResult.Predictions[0].BoundingBox.Height * detectedImage.Height));

                        detectedImage.Save(detectedImagePath, ImageFormat.Png);
                    }

                    ImageFileUrl = "/images/detectedImage.png";

                }
                else
                {
                    Result = "判定できませんでした";
                }
            }
            catch (CustomVisionErrorException e)
            {
                Result = "エラー: " + e.Message;
            }

            return Page();

        }
    }
}