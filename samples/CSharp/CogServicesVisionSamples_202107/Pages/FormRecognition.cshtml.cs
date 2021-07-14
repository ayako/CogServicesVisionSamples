using System;
using System.IO;
using System.Threading.Tasks;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CogServicesVisionSamples_202107.Pages
{
    public class FormRecognitionModel : PageModel
    {
        [BindProperty]
        //Input from web page
        public IFormFile ImageFile { get; set; }

        //Output to web page
        public string ImageFileUrl { get; set; }
        public string Result { get; set; }

        public RecognizedForm RecognizedForm { get; set; }

        // Setting for using FormRecognizer
        private const string frKey = "YOUR_FORMRECOGNIZER_KEY";
        private const string frEndpoint = "https://YOUR_LOCATION.api.cognitive.microsoft.com";
        private const string frModelId = "YOUR_FORMRECOGNIZER_MODELID";

        // Setting for FileIO
        private readonly IWebHostEnvironment _host;
        public FormRecognitionModel(IWebHostEnvironment host)
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

            // Post image to Form Recognizer, get resut and show on web page
            var frClient = new FormRecognizerClient(new Uri(frEndpoint), new Azure.AzureKeyCredential(frKey));

            try
            {
                var frOperation = await frClient.StartRecognizeCustomFormsAsync(frModelId, ImageFile.OpenReadStream());
                var frResponse = await frOperation.WaitForCompletionAsync();

                if (frResponse.Value != null)
                {
                    Result = "結果:";

                    RecognizedForm = frResponse.Value[0];

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

    }
}
