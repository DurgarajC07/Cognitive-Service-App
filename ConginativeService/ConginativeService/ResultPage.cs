using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ConginativeService
{
    public class ResultPage : ContentPage
    {
        private Image _imageCap;
        private Label _imageInfo;
        List<string> _output;
        FormattedString fs;
        public ResultPage(List<string> output, ImageSource data)
        {
            NavigationPage.SetHasBackButton(this, true);
            _output = output;
            _imageCap = new Image
            {
                WidthRequest = 300,
                HeightRequest = 300,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Source = data
            };
            fs = new FormattedString();
            _imageInfo = new Label
            {
                TextColor = Color.Black,
                FontSize = 16,
                Margin= new Thickness(20,10),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FormattedText = fs
            };
            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Children = {
                    _imageCap,
                    _imageInfo
                }
                }
            };
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            foreach (var item in _output)
            {
                fs.Spans.Add(new Span { Text = Environment.NewLine });
                fs.Spans.Add(new Span { Text = item });
            }
        }

    }
}