using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
//using Azure.AI.FormRecognizer;
//using Azure.AI.FormRecognizer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CogServicesVisionSamples_202103.Pages
{
    public class FormRecognitionModel : PageModel
    {
        [BindProperty]
        //Input from web page
        public IFormFile ImageFile { get; set; }

        //Output to web page
        public string ImageFileUrl { get; set; }
        public string Result { get; set; }

        //public RecognizedForm RecognizedForm { get; set; }
        public DocumentResult RecognizedForm { get; set; }

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

        static readonly HttpClient httpClient = new HttpClient();


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
            //var frClient = new FormRecognizerClient(new Uri(frEndpoint), new Azure.AzureKeyCredential(frKey));

            try
            {
                //var frOperation = await frClient.StartRecognizeCustomFormsAsync(frModelId, ImageFile.OpenReadStream());
                //var frResponse = await frOperation.WaitForCompletionAsync();
                //if (frResponse.Value != null)
                //{
                //    Result = "����:";
                //    RecognizedForm = frResponse.Value[0];
                //}
                //else
                //{
                //    Result = "�ǂݎ��ł��܂���ł���";
                //}

                // Call API direct without using Form Recognizer Libraries
                var content = new StreamContent(ImageFile.OpenReadStream());
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                var url = frEndpoint + "formrecognizer/v2.1-preview.3/custom/models/" + frModelId + "/analyze";

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add(@"Ocp-Apim-Subscription-Key", frKey);
                request.Content = content;
                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var resultUrl = (string[])response.Headers.GetValues("Operation-Location");

                    var frStatus = string.Empty;
                    var frResponseJson = string.Empty;

                    while (frStatus != "succeeded")
                    {
                        await Task.Delay(1000);

                        request = new HttpRequestMessage(HttpMethod.Get, resultUrl[0]);
                        request.Headers.Add(@"Ocp-Apim-Subscription-Key", frKey);
                        response = await httpClient.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            frResponseJson = await response.Content.ReadAsStringAsync();
                            frStatus = JsonSerializer.Deserialize<TempCustomIdFormResult>(frResponseJson).status;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(frResponseJson))
                    {
                        var frResponse = JsonSerializer.Deserialize<CustomIdFormResult>(frResponseJson);

                        Result = "結果";
                        RecognizedForm = frResponse.analyzeResult.documentResults[0];
                    }
                }

                if (string.IsNullOrEmpty(Result))
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


        // Custom Class for Cusom Form Reccognition

        public class TempCustomIdFormResult
        {
            public string status { get; set; }
            public DateTime createdDateTime { get; set; }
            public DateTime lastUpdatedDateTime { get; set; }
            public object analyzeReult { get; set; }
        }


        public class CustomIdFormResult
        {
            public string status { get; set; }
            public DateTime createdDateTime { get; set; }
            public DateTime lastUpdatedDateTime { get; set; }
            public AnalyzeResult analyzeResult { get; set; }
        }

        public class AnalyzeResult
        {
            public string version { get; set; }
            public object readResults { get; set; }
            public object pageResults { get; set; }
            public DocumentResult[] documentResults { get; set; }
            public object errors { get; set; }
        }

        public class DocumentResult
        {
            public string docType { get; set; }
            public string modelId { get; set; }
            public int[] pageRange { get; set; }
            public Fields fields { get; set; }
            public float docTypeConfidence { get; set; }
        }

        public class Fields
        {
            public LicenseNo LicenseNo { get; set; }
            public ExpirationDate ExpirationDate { get; set; }
            public Name Name { get; set; }
            public Birthday Birthday { get; set; }
            public Address Address { get; set; }
        }

        public class LicenseNo
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public float[] boundingBox { get; set; }
            public float confidence { get; set; }
        }

        public class ExpirationDate
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public float[] boundingBox { get; set; }
            public float confidence { get; set; }
        }

        public class Name
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public float[] boundingBox { get; set; }
            public float confidence { get; set; }
        }

        public class Birthday
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public float[] boundingBox { get; set; }
            public float confidence { get; set; }
        }

        public class Address
        {
            public string type { get; set; }
            public string valueString { get; set; }
            public string text { get; set; }
            public int page { get; set; }
            public float[] boundingBox { get; set; }
            public float confidence { get; set; }
        }

    }
}
