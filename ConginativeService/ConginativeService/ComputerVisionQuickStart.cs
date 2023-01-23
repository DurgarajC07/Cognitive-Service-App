using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
namespace ConginativeService
{
    public class ComputerVisionQuickStart
    {
        public static string subscriptionVisionKey = "17c58a92b553421c87268a4c50756dd3";
        public static string visionEndpoint = "https://docrdetect.cognitiveservices.azure.com/";

        public static string subscriptionFaceKey = "2da5accfbba34bd9866b2a578e8a40ef";
        public static string faceEndpoint = "https://durgafacedetect.cognitiveservices.azure.com/";

        public static string subscriptionSpeechKey = "d2c814c6189a49a19875d88476441d0d";
        public static string speechEndpoint = "https://eastus.api.cognitive.microsoft.com/sts/v1.0/issuetoken";

        // Used for all examples.
        // URL for the images.
        public static string IMAGE_BASE_URL = "https://upload.wikimedia.org/wikipedia/commons/f/f1/An_example_for_a_nailed_note.jpg";
        // Used for all examples.
        // URL for the images.
        public static string IMAGE_FACE_BASE_URL = "https://csdx.blob.core.windows.net/resources/Face/Images/";
        public static string RECOGNITION_MODEL4 = RecognitionModel.Recognition04;
    }
}
