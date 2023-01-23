using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConginativeService
{
    class FaceAPI
    {
        static string personGroupId = Guid.NewGuid().ToString();
        public static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }
        /* 
 * DETECT FACES
 * Detects features from faces and IDs them.
 */
        public static async Task DetectFaceExtract(IFaceClient client, string url, string stream, string recognitionModel, List<string> OutputResult)
        {
            Console.WriteLine("========DETECT FACES========");
            Console.WriteLine();
            IList<DetectedFace> detectedFaces;
            if (string.IsNullOrWhiteSpace(url))
            {
                using (Stream imageFileStream = File.OpenRead(stream))
                {
                    detectedFaces = await client.Face.DetectWithStreamAsync(imageFileStream,
                  returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Accessories, FaceAttributeType.Age,
                FaceAttributeType.Blur, FaceAttributeType.Emotion, FaceAttributeType.Exposure, FaceAttributeType.FacialHair,
                FaceAttributeType.Gender, FaceAttributeType.Glasses, FaceAttributeType.Hair, FaceAttributeType.HeadPose,
                FaceAttributeType.Makeup, FaceAttributeType.Noise, FaceAttributeType.Occlusion, FaceAttributeType.Smile },
                  // We specify detection model 1 because we are retrieving attributes.
                  detectionModel: DetectionModel.Detection01,
                  recognitionModel: recognitionModel);
                }
            }
            else
            {
                detectedFaces = await client.Face.DetectWithUrlAsync(url,
                   returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Accessories, FaceAttributeType.Age,
                FaceAttributeType.Blur, FaceAttributeType.Emotion, FaceAttributeType.Exposure, FaceAttributeType.FacialHair,
                FaceAttributeType.Gender, FaceAttributeType.Glasses, FaceAttributeType.Hair, FaceAttributeType.HeadPose,
                FaceAttributeType.Makeup, FaceAttributeType.Noise, FaceAttributeType.Occlusion, FaceAttributeType.Smile },
                   // We specify detection model 1 because we are retrieving attributes.
                   detectionModel: DetectionModel.Detection01,
                   recognitionModel: recognitionModel);
            }
            //OutputResult.Add($"{detectedFaces.Count} face(s) detected from image `{imageFileName}`.");
            // Parse and print all attributes of each detected face.
            foreach (var face in detectedFaces)
            {
                //OutputResult.Add($"Face attributes for {imageFileName}:");
                // Get bounding box of the faces
                OutputResult.Add($"Rectangle(Left/Top/Width/Height) : {face.FaceRectangle.Left} {face.FaceRectangle.Top} {face.FaceRectangle.Width} {face.FaceRectangle.Height}");
                // Get accessories of the faces
                List<Accessory> accessoriesList = (List<Accessory>)face.FaceAttributes.Accessories;
                int count = face.FaceAttributes.Accessories.Count;
                string accessory; string[] accessoryArray = new string[count];
                if (count == 0) { accessory = "NoAccessories"; }
                else
                {
                    for (int i = 0; i < count; ++i) { accessoryArray[i] = accessoriesList[i].Type.ToString(); }
                    accessory = string.Join(",", accessoryArray);
                }
                OutputResult.Add($"Accessories : {accessory}");

                // Get face other attributes
                OutputResult.Add($"Age : {face.FaceAttributes.Age}");
                OutputResult.Add($"Blur : {face.FaceAttributes.Blur.BlurLevel}");
                // Get emotion on the face
                string emotionType = string.Empty;
                double emotionValue = 0.0;
                Emotion emotion = face.FaceAttributes.Emotion;
                if (emotion.Anger > emotionValue) { emotionValue = emotion.Anger; emotionType = "Anger"; }
                if (emotion.Contempt > emotionValue) { emotionValue = emotion.Contempt; emotionType = "Contempt"; }
                if (emotion.Disgust > emotionValue) { emotionValue = emotion.Disgust; emotionType = "Disgust"; }
                if (emotion.Fear > emotionValue) { emotionValue = emotion.Fear; emotionType = "Fear"; }
                if (emotion.Happiness > emotionValue) { emotionValue = emotion.Happiness; emotionType = "Happiness"; }
                if (emotion.Neutral > emotionValue) { emotionValue = emotion.Neutral; emotionType = "Neutral"; }
                if (emotion.Sadness > emotionValue) { emotionValue = emotion.Sadness; emotionType = "Sadness"; }
                if (emotion.Surprise > emotionValue) { emotionType = "Surprise"; }
                OutputResult.Add($"Emotion : {emotionType}");
                // Get more face attributes
                OutputResult.Add($"Exposure : {face.FaceAttributes.Exposure.ExposureLevel}");
                OutputResult.Add($"FacialHair : {string.Format("{0}", face.FaceAttributes.FacialHair.Moustache + face.FaceAttributes.FacialHair.Beard + face.FaceAttributes.FacialHair.Sideburns > 0 ? "Yes" : "No")}");
                OutputResult.Add($"Gender : {face.FaceAttributes.Gender}");
                OutputResult.Add($"Glasses : {face.FaceAttributes.Glasses}");
                // Get hair color
                Hair hair = face.FaceAttributes.Hair;
                string color = null;
                if (hair.HairColor.Count == 0) { if (hair.Invisible) { color = "Invisible"; } else { color = "Bald"; } }
                HairColorType returnColor = HairColorType.Unknown;
                double maxConfidence = 0.0f;
                foreach (HairColor hairColor in hair.HairColor)
                {
                    if (hairColor.Confidence <= maxConfidence) { continue; }
                    maxConfidence = hairColor.Confidence; returnColor = hairColor.Color; color = returnColor.ToString();
                }
                OutputResult.Add($"Hair : {color}");
                // Get more attributes
                OutputResult.Add($"Head Position : {string.Format("Pitch: {0}, Roll: {1}, Yaw: {2}", Math.Round(face.FaceAttributes.HeadPose.Pitch, 2), Math.Round(face.FaceAttributes.HeadPose.Roll, 2), Math.Round(face.FaceAttributes.HeadPose.Yaw, 2))}");
                OutputResult.Add($"Makeup : {string.Format("{0}", (face.FaceAttributes.Makeup.EyeMakeup || face.FaceAttributes.Makeup.LipMakeup) ? "Yes" : "No")}");
                OutputResult.Add($"Noise : {face.FaceAttributes.Noise.NoiseLevel}");
                OutputResult.Add($"Occlusion : {string.Format("EyeOccluded: {0}", face.FaceAttributes.Occlusion.EyeOccluded ? "Yes" : "No")} " +
                    $" {string.Format("ForeheadOccluded: {0}", face.FaceAttributes.Occlusion.ForeheadOccluded ? "Yes" : "No")}   {string.Format("MouthOccluded: {0}", face.FaceAttributes.Occlusion.MouthOccluded ? "Yes" : "No")}");
                OutputResult.Add($"Smile : {face.FaceAttributes.Smile}");
                Console.WriteLine();
                //}
            }
        }
        public static async Task IdentifyInPersonGroup(IFaceClient client, string url, string recognitionModel)
        {
            Console.WriteLine("========IDENTIFY FACES========");
            Console.WriteLine();
            // Create a dictionary for all your images, grouping similar ones under the same key.
            Dictionary<string, string[]> personDictionary =
                new Dictionary<string, string[]>
                    { { "Family1-Dad", new[] { "Family1-Dad1.jpg", "Family1-Dad2.jpg" } },
              { "Family1-Mom", new[] { "Family1-Mom1.jpg", "Family1-Mom2.jpg" } },
              { "Family1-Son", new[] { "Family1-Son1.jpg", "Family1-Son2.jpg" } },
              { "Family1-Daughter", new[] { "Family1-Daughter1.jpg", "Family1-Daughter2.jpg" } },
              { "Family2-Lady", new[] { "Family2-Lady1.jpg", "Family2-Lady2.jpg" } },
              { "Family2-Man", new[] { "Family2-Man1.jpg", "Family2-Man2.jpg" } }
                    };
            // A group photo that includes some of the persons you seek to identify from your dictionary.
            string sourceImageFileName = "identification1.jpg";
            // Create a person group. 
            Console.WriteLine($"Create a person group ({personGroupId}).");
            await client.PersonGroup.CreateAsync(personGroupId, personGroupId, recognitionModel: recognitionModel);
            // The similar faces will be grouped into a single person group person.
            foreach (var groupedFace in personDictionary.Keys)
            {
                // Limit TPS
                await Task.Delay(250);
                Person person = await client.PersonGroupPerson.CreateAsync(personGroupId: personGroupId, name: groupedFace);
                Console.WriteLine($"Create a person group person '{groupedFace}'.");

                // Add face to the person group person.
                foreach (var similarImage in personDictionary[groupedFace])
                {
                    Console.WriteLine($"Add face to the person group person({groupedFace}) from image `{similarImage}`");
                    PersistedFace face = await client.PersonGroupPerson.AddFaceFromUrlAsync(personGroupId, person.PersonId,
                        $"{url}{similarImage}", similarImage);
                }
            }
            // Start to train the person group.
            Console.WriteLine();
            Console.WriteLine($"Train person group {personGroupId}.");
            await client.PersonGroup.TrainAsync(personGroupId);
            // Wait until the training is completed.
            while (true)
            {
                await Task.Delay(1000);
                var trainingStatus = await client.PersonGroup.GetTrainingStatusAsync(personGroupId);
                Console.WriteLine($"Training status: {trainingStatus.Status}.");
                if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
            }
            Console.WriteLine();
            // Start to train the person group.
            Console.WriteLine();
            Console.WriteLine($"Train person group {personGroupId}.");
            await client.PersonGroup.TrainAsync(personGroupId);
            // Wait until the training is completed.
            while (true)
            {
                await Task.Delay(1000);
                var trainingStatus = await client.PersonGroup.GetTrainingStatusAsync(personGroupId);
                Console.WriteLine($"Training status: {trainingStatus.Status}.");
                if (trainingStatus.Status == TrainingStatusType.Succeeded) { break; }
            }
            Console.WriteLine();
            List<Guid> sourceFaceIds = new List<Guid>();
            // Detect faces from source image url.
            List<DetectedFace> detectedFaces = await DetectFaceRecognize(client, $"{url}{sourceImageFileName}", recognitionModel);
            // Add detected faceId to sourceFaceIds.
            foreach (var detectedFace in detectedFaces) { sourceFaceIds.Add(detectedFace.FaceId.Value); }
            // Identify the faces in a person group. 
            var identifyResults = await client.Face.IdentifyAsync(sourceFaceIds, personGroupId);
            foreach (var identifyResult in identifyResults)
            {
                Person person = await client.PersonGroupPerson.GetAsync(personGroupId, identifyResult.Candidates[0].PersonId);
                Console.WriteLine($"Person '{person.Name}' is identified for face in: {sourceImageFileName} - {identifyResult.FaceId}," +
                    $" confidence: {identifyResult.Candidates[0].Confidence}.");
            }
            Console.WriteLine();
        }
        private static async Task<List<DetectedFace>> DetectFaceRecognize(IFaceClient faceClient, string url, string recognition_model)
        {
            // Detect faces from image URL. Since only recognizing, use the recognition model 1.
            // We use detection model 3 because we are not retrieving attributes.
            IList<DetectedFace> detectedFaces = await faceClient.Face.DetectWithUrlAsync(url, recognitionModel: recognition_model, detectionModel: DetectionModel.Detection03);
            Console.WriteLine($"{detectedFaces.Count} face(s) detected from image `{Path.GetFileName(url)}`");
            return detectedFaces.ToList();
        }
        /*
 * FIND SIMILAR
 * This example will take an image and find a similar one to it in another image.
 */
        public static async Task FindSimilar(IFaceClient client, string url, string recognition_model)
        {
            Console.WriteLine("========FIND SIMILAR========");
            Console.WriteLine();
            List<string> targetImageFileNames = new List<string>
                        {
                            "Family1-Dad1.jpg",
                            "Family1-Daughter1.jpg",
                            "Family1-Mom1.jpg",
                            "Family1-Son1.jpg",
                            "Family2-Lady1.jpg",
                            "Family2-Man1.jpg",
                            "Family3-Lady1.jpg",
                            "Family3-Man1.jpg"
                        };
            string sourceImageFileName = "findsimilar.jpg";
            IList<Guid?> targetFaceIds = new List<Guid?>();
            foreach (var targetImageFileName in targetImageFileNames)
            {
                // Detect faces from target image url.
                var faces = await DetectFaceRecognize(client, $"{url}{targetImageFileName}", recognition_model);
                // Add detected faceId to list of GUIDs.
                targetFaceIds.Add(faces[0].FaceId.Value);
            }
            // Detect faces from source image url.
            IList<DetectedFace> detectedFaces = await DetectFaceRecognize(client, $"{url}{sourceImageFileName}", recognition_model);
            Console.WriteLine();

            // Find a similar face(s) in the list of IDs. Comapring only the first in list for testing purposes.
            IList<SimilarFace> similarResults = await client.Face.FindSimilarAsync(detectedFaces[0].FaceId.Value, null, null, targetFaceIds);

            foreach (var similarResult in similarResults)
            {
                Console.WriteLine($"Faces from {sourceImageFileName} & ID:{similarResult.FaceId} are similar with confidence: {similarResult.Confidence}.");
            }
            Console.WriteLine();
            // At end, delete person groups in both regions (since testing only)
            Console.WriteLine("========DELETE PERSON GROUP========");
            Console.WriteLine();
            DeletePersonGroup(client, personGroupId).Wait();
        }
        public static async Task DeletePersonGroup(IFaceClient client, String personGroupId)
        {
            await client.PersonGroup.DeleteAsync(personGroupId);
            Console.WriteLine($"Deleted the person group {personGroupId}.");
        }
    }
}
