using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Metro_Nexterlizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NexterlizerPage : Page
    {
        public static readonly DependencyProperty HasSuggestionProperty = DependencyProperty.Register("HasSuggestion", typeof(bool), typeof(NexterlizerPage), null);

        public static readonly DependencyProperty WithoutSuggestionProperty = DependencyProperty.Register("WithoutSuggestion", typeof(bool), typeof(NexterlizerPage), null);

        public static readonly DependencyProperty SuggestionTextProperty = DependencyProperty.Register("SuggestionText", typeof(string), typeof(NexterlizerPage), new PropertyMetadata(string.Empty, OnSuggestionTextChanged));

        public bool HasSuggestion
        {
            get { return (bool)GetValue(HasSuggestionProperty); }
            set { SetValue(HasSuggestionProperty, value); }
        }

        public bool WithoutSuggestion
        {
            get { return (bool)GetValue(WithoutSuggestionProperty); }
            set { SetValue(WithoutSuggestionProperty, value); }
        }

        public string SuggestionText
        {
            get { return (string)GetValue(SuggestionTextProperty); }
            set { SetValue(SuggestionTextProperty, value); }
        }

        public Uri SuggestionUri { get; set; }

        public NexterlizerPage()
        {
            this.InitializeComponent();
            NexterlizerPage.SetSuggestionBools(this.SuggestionText, this);
            this.DataContext = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private static void OnSuggestionTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var suggestionText = (string)args.NewValue;
            NexterlizerPage us = (NexterlizerPage)obj;
            SetSuggestionBools(suggestionText, us);
        }

        private static void SetSuggestionBools(string suggestionText, NexterlizerPage us)
        {
            us.HasSuggestion = !string.IsNullOrEmpty(suggestionText);
            us.WithoutSuggestion = string.IsNullOrEmpty(suggestionText);
        }

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            // TODO use commands
            var secretAccessKey = "1234567890";
            var accessKey = "AKIAIOSFODNN7EXAMPLE";
            IAmazonProductAdvertisingApi amz = new AmazonProductAdvertisingApi(secretAccessKey, accessKey);
            var xml = await amz.CallAmazon(this.SearchText.Text);
            this.SuggestionText = amz.GetSuggestionTextFromResponse(xml);
            this.SuggestionUri = amz.GetSuggestionUriFromResponse(xml);
        }

        private async void SuggestionHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(this.SuggestionUri);
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            ////NexterlizerPage.SetSuggestionBools(this.SearchText.Text, this);
        }
    }
}
