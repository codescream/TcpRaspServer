using DemoHarnessUpd;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ImageAnalyze
{
    class CompVision
    {
        // subscriptionKey = "0123456789abcdef0123456789ABCDEF"
        private static string subscriptionKey = Environment.GetEnvironmentVariable("azure_CV_Key", EnvironmentVariableTarget.User);

        static string text = "";
        static string visual = "";
        //static string pretext = "This picture shows ";
        //static string midtext = " with a text that says ";

        // For printed text, change to TextRecognitionMode.Printed
        private const TextRecognitionMode textRecognitionMode =
            TextRecognitionMode.Handwritten;

        // localImagePath = @"C:\Documents\LocalImage.jpg"
        private static string localImagePath = @"C:\Users\ogilo\Documents\image.jpg";

        //private const string remoteImageUrl =
        //    "http://upload.wikimedia.org/wikipedia/commons/3/3c/Shaki_waterfall.jpg";

        // localImagePath = @"C:\Documents\LocalImage.jpg"
        //private const string localImagePath2 = @"C:\Users\ogilo\Documents\locattext.jpg";

        /*private const string remoteImageUrl2 =
            "https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/" +
            "Cursive_Writing_on_Notebook_paper.jpg/" +
            "800px-Cursive_Writing_on_Notebook_paper.jpg";*/

        private const int numberOfCharsInOperationId = 36;

        // Specify the features to return
        private static readonly List<VisualFeatureTypes> features =
            new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags
        };

        public string AnalyzeImage()
        {
            using (var fileStream = new FileStream(@"locattext.jpg", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                string search = fileStream.Name;
                localImagePath = search;
                /*if(count > 0)
                {
                    count--;
                    File.Create(@"sample" + count + ".wav").Close();
                    System.IO.File.Delete(@"sample" + count + ".wav");
                }*/

                //count++;
            }
            

            ComputerVisionClient computerVision = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            // You must use the same region as you used to get your subscription
            // keys. For example, if you got your subscription keys from westus,
            // replace "westcentralus" with "westus".
            //
            // Free trial subscription keys are generated in the "westus"
            // region. If you use a free trial subscription key, you shouldn't
            // need to change the region.

            // Specify the Azure region
            computerVision.Endpoint = "https://westus.api.cognitive.microsoft.com/";

            Console.WriteLine("Images being analyzed ...");
            //var t1 = AnalyzeRemoteAsync(computerVision, remoteImageUrl);
            var t2 = AnalyzeLocalAsync(computerVision, localImagePath);
            //var t3 = ExtractRemoteTextAsync(computerVision, remoteImageUrl2);
            var t4 = ExtractLocalTextAsync(computerVision, localImagePath);

            Task.WhenAll(t2, t4).Wait(20000);
            //Console.WriteLine("Press ENTER to exit");
            //Console.ReadLine();

            //string fulltext =  + visual +  + text;

            //return "This picture shows "+visual+" with a text that says "+text;
            return "This picture shows " + visual + " with a text that says; " + text;
        }

        // Analyze a remote image
        /*private static async Task AnalyzeRemoteAsync(ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine(
                    "\nInvalid remoteImageUrl:\n{0} \n", imageUrl);
                return;
            }

            ImageAnalysis analysis =
                await computerVision.AnalyzeImageAsync(imageUrl, features);
            DisplayResults(analysis, imageUrl);
        }*/

        // Analyze a local image
        private static async Task AnalyzeLocalAsync(ComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine(
                    "\nUnable to open or read localImagePath:\n{0} \n", imagePath);
                return;
            }

            using (Stream imageStream = File.OpenRead(imagePath))
            {
                ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(
                    imageStream, features);
                DisplayResults(analysis, imagePath);
            }
        }

        // Recognize text from a remote image
        /*private static async Task ExtractRemoteTextAsync(ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine(
                    "\nInvalid remoteImageUrl:\n{0} \n", imageUrl);
                return;
            }

            // Start the async process to recognize the text
            RecognizeTextHeaders textHeaders =
                await computerVision.RecognizeTextAsync(
                    imageUrl, textRecognitionMode);

            await GetTextAsync(computerVision, textHeaders.OperationLocation);
        }*/

        // Recognize text from a local image
        private static async Task ExtractLocalTextAsync(ComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine(
                    "\nUnable to open or read localImagePath:\n{0} \n", imagePath);
                return;
            }

            using (Stream imageStream = File.OpenRead(imagePath))
            {
                // Start the async process to recognize the text
                RecognizeTextInStreamHeaders textHeaders =
                    await computerVision.RecognizeTextInStreamAsync(
                        imageStream, textRecognitionMode);

                text = await GetTextAsync(computerVision, textHeaders.OperationLocation);
            }
        }

        // Retrieve the recognized text
        private static async Task<string> GetTextAsync(ComputerVisionClient computerVision, string operationLocation)
        {
            string completeText = "";
            // Retrieve the URI where the recognized text will be
            // stored from the Operation-Location header
            string operationId = operationLocation.Substring(
                operationLocation.Length - numberOfCharsInOperationId);

            Console.WriteLine("\nCalling GetHandwritingRecognitionOperationResultAsync()");
            TextOperationResult result =
                await computerVision.GetTextOperationResultAsync(operationId);

            // Wait for the operation to complete
            int i = 0;
            int maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                Console.WriteLine(
                    "Server status: {0}, waiting {1} seconds...", result.Status, i);
                await Task.Delay(1000);

                result = await computerVision.GetTextOperationResultAsync(operationId);
            }

            // Display the results
            Console.WriteLine();
            var lines = result.RecognitionResult.Lines;
            foreach (Line line in lines)
            {
                Console.WriteLine(line.Text);
                completeText += line.Text + " ";
            }
            //Entry entry = new Entry();
            Console.WriteLine();

            text = completeText;

            return text;
        }

        // Display the most relevant caption for the image
        private static void DisplayResults(ImageAnalysis analysis, string imageUri)
        {
            Console.WriteLine(imageUri);
            Console.WriteLine(analysis.Description.Captions[0].Text + "\n");
            visual = analysis.Description.Captions[0].Text;
        }
    }
}