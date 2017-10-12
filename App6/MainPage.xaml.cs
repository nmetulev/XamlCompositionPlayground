using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App6
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Random random = new Random();
        public MainPage()
        {
            this.InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var visual = ElementCompositionPreview.GetElementVisual(Element2);
            visual.CenterPoint = new Vector3(50f, 50f, 1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Element.Visibility = Element.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Canvas.SetTop(Element2, random.NextDouble() * Window.Current.Bounds.Height);
            Canvas.SetLeft(Element2, random.NextDouble() * Window.Current.Bounds.Width);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var visual = ElementCompositionPreview.GetElementVisual(Element2);
            visual.Scale = new Vector3((float)random.NextDouble() * 2,
                                       (float)random.NextDouble() * 2,
                                       1);
        }

        private void ConnectedClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Second_Page));
        }

        private void DetailsPageClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DetailsPage));
        }
    }
}
