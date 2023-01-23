using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConginativeService
{
    public class VisionAPI
    {
        internal static async  Task AnalyzeImageUrl(ComputerVisionClient visionClient, string photoPath, Stream streamData, List<string> outputResult)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - URL");
            Console.WriteLine();

            // Creating a list that defines the features to be extracted from the image. 

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Tags
            };

            Console.WriteLine($"Analyzing the image {Path.GetFileName(photoPath)}...");
            Console.WriteLine();
            // Analyze the URL image 
         //   ImageAnalysis results = await visionClient.AnalyzeImageAsync(photoPath, visualFeatures: features);
            ImageAnalysis streamDataResult = await visionClient.AnalyzeImageInStreamAsync(streamData, visualFeatures: features);
            // Image tags and their confidence score
            Console.WriteLine("Tags:");
            //foreach (var tag in results.Tags)
            //{
            //    outputResult.Add($"image tag{tag.Name} image confidence {tag.Confidence}");
            //}
            foreach (var tag in streamDataResult.Tags)
            {
                outputResult.Add($"{tag.Name}  {tag.Confidence}");
            }
            Console.WriteLine();
        }

        internal static async Task OCRFromStreamAsync(Stream streamData, string endpoint, string subscriptionVisionKey, List<string> outputResult)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("READ FILE FROM URL");
            Console.WriteLine();
            ComputerVisionClient client = Authenticate(endpoint, subscriptionVisionKey);
            
            // Read text from URL
            var textHeaders = await client.ReadInStreamAsync(streamData);
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            //Console.WriteLine($"Extracting text from URL file {Path.GetFileName(streamData)}...");
            Console.WriteLine();
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            Console.WriteLine();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    outputResult.Add(line.Text);
                    Console.WriteLine(line.Text);
                }
            }
            Console.WriteLine();
        }
   

        internal static ComputerVisionClient Authenticate(string visionEndpoint, string subscriptionVisionKey)
        {
            ComputerVisionClient client =
             new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionVisionKey))
             { Endpoint = visionEndpoint };
            return client;
        }
    }
}
