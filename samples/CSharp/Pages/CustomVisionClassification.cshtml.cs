using System;
using System.Collections.Generic;
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
    public class CustomVisionClassificationModel : PageModel
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
        public CustomVisionClassificationModel(IHostingEnvironment host)
        {
            _host = host;
        }

        //public void OnGet()
        //{

        //}

        public async Task<IActionResult> OnPostAsync()
        {
            // Show image on web page
            var imageFilePath = Path.Combine(_host.WebRootPath, "images\\uploadedImage.jpg");
            using (var fileStream = new FileStream(imageFilePath, FileMode.OpenOrCreate))
            {
                await ImageFile.CopyToAsync(fileStream);
            }
            ImageFileUrl = "/images/uploadedImage.jpg";

            // Post image to Custom Vision, get resut and show on web page
            var cvClient = new CustomVisionPredictionClient()
            {
                Endpoint = cvEndpoint,
                ApiKey = cvPredictionKey
            };
            
            try
            {
                var cvResult = await cvClient.ClassifyImageAsync(
                    Guid.Parse(cvProjectId), cvPublishName, ImageFile.OpenReadStream());
                if (cvResult.Predictions.Count > 0)
                {
                    Result = "結果:";
                    Predictions = cvResult.Predictions;
                }
                else
                {
                    Result = "判定できませんでした";
                }
            }
            catch(CustomVisionErrorException e)
            {
                Result = "エラー: " + e.Message;
            }

            return Page();
        }

    }
}