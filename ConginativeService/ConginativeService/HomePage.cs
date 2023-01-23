using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ConginativeService
{
    public class HomePage : ContentPage
    {
        private IFaceClient _faceClient;
        private ComputerVisionClient _visionClient;
        public string PhotoPath { get; set; }
        public FileResult Result { get; set; }
        public ImageSource DataSource { get; set; }
        public List<string> OutputResult { get; set; }
        public Stream StreamData { get; set; }
        private Button _OCR;
        private Button _ImageAnalytics;
        private Button _FaceDetection;
        private Button _FaceAnaLysis;
        public HomePage()
        {
            _OCR = new Button
            {
                Text = "OCR"
            };
            _OCR.Clicked += _OCR_Clicked;
            _ImageAnalytics = new Button
            {
                Text = "Image Analytics"
            };
            _ImageAnalytics.Clicked += _ImageAnalytics_Clicked; ;
            _FaceDetection = new Button
            {
                Text = "Face Detection"
            };
            _FaceDetection.Clicked += _FaceDetection_Clicked; ;
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Welcome to Xamarin.Forms!" },
                    _OCR,
                    _ImageAnalytics,
                    _FaceDetection
                }
            };
        }

        private async void _FaceDetection_Clicked(object sender, EventArgs e)
        {
            await TakePhotoAsync();
            OutputResult = new List<string>();
            await FaceAPI.DetectFaceExtract(_faceClient, "", PhotoPath, ComputerVisionQuickStart.RECOGNITION_MODEL4, OutputResult);
            await Navigation.PushModalAsync(new NavigationPage(new ResultPage(OutputResult, DataSource)));

        }
        private async void _ImageAnalytics_Clicked(object sender, EventArgs e)
        {
            await TakePhotoAsync();
            OutputResult = new List<string>();
            await VisionAPI.AnalyzeImageUrl(_visionClient, PhotoPath, StreamData, OutputResult);
            await Navigation.PushModalAsync(new NavigationPage(new ResultPage(OutputResult, DataSource)));
        }
        private async void _OCR_Clicked(object sender, EventArgs e)
        {
            await TakePhotoAsync();
            OutputResult = new List<string>();
            //  await VisionAPI.ReadFileUrl(_visionClient, ComputerVisionQuickstart.IMAGE_BASE_URL, StreamData);
            await VisionAPI.OCRFromStreamAsync(StreamData, ComputerVisionQuickStart.visionEndpoint, ComputerVisionQuickStart.subscriptionVisionKey, OutputResult);

            await Navigation.PushModalAsync(new NavigationPage(new ResultPage(OutputResult, DataSource)));
        }
        public async Task<PermissionStatus> CheckAndRequestCameraPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status == PermissionStatus.Granted)
                return status;
            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }
            if (Permissions.ShouldShowRationale<Permissions.Camera>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }
            status = await Permissions.RequestAsync<Permissions.Camera>();
            return status;
        }
        public async Task<PermissionStatus> CheckAndRequestStoragePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            var status1 = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status == PermissionStatus.Granted && status1 == PermissionStatus.Granted)
                return status;
            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }
            if (Permissions.ShouldShowRationale<Permissions.StorageWrite>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }
            status = await Permissions.RequestAsync<Permissions.StorageWrite>();

            return status;
        }
        public async Task TakePhotoAsync()
        {
            try
            {
                if ((await CheckAndRequestCameraPermission()) == PermissionStatus.Granted && (await CheckAndRequestStoragePermission()) == PermissionStatus.Granted)
                {
                    //Result = await MediaPicker.CapturePhotoAsync();
                    //await LoadPhotoAsync(Result);
                    //Console.WriteLine($"CapturePhotoAsync COMPLETED: {PhotoPath}");
                    if (CrossMedia.Current.IsTakePhotoSupported)
                    {
                        var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            Directory = "Photographs",
                            SaveToAlbum = true,
                            CompressionQuality = 40,
                            CustomPhotoSize = 60,
                            PhotoSize = PhotoSize.MaxWidthHeight,
                            MaxWidthHeight = 2000,
                            DefaultCamera = CameraDevice.Rear
                        }).ConfigureAwait(true);
                        if (file != null)
                        {
                            StreamData = file.GetStream();
                            DataSource = ImageSource.FromStream(() =>
                            {
                                var stream = file.GetStream();
                                return stream;
                            });
                            PhotoPath = file.Path;
                        }
                    }
                    else
                    {
                        await DisplayAlert("Not Supported", "Your device does not support this feature.", "OK. Understood")
                            .ConfigureAwait(true);
                    }
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }
        public async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
            {
                PhotoPath = null;
                return;
            }
            // save the file into local storage
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            {
                using (var newStream = File.OpenWrite(newFile))
                {
                    await stream.CopyToAsync(newStream);
                    //   StreamData = stream;
                }
            }
            PhotoPath = newFile;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await CrossMedia.Current.Initialize();
            _faceClient = FaceAPI.Authenticate(ComputerVisionQuickStart.faceEndpoint, ComputerVisionQuickStart.subscriptionFaceKey);
            _visionClient = VisionAPI.Authenticate(ComputerVisionQuickStart.visionEndpoint, ComputerVisionQuickStart.subscriptionVisionKey);
        }
    }
}
