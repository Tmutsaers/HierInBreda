using HierInBreda.Control;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HierInBreda.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartMenu : Page
    {
        private int dutchFlagCount;
        private int englishFlagCount;
        private MainControl mainControl;
        private DispatcherTimer timer;

        public StartMenu()
        {
            this.InitializeComponent();
            mainControl = new MainControl();
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Start();
        }

        void timer_Tick(object sender, object e)
        {
            dutchFlagCount++;
            englishFlagCount++;
            if (dutchFlagCount >= 87)
                dutchFlagCount = 0;
            if (englishFlagCount >= 37)
                englishFlagCount = 0;
            flagDutch.Source = new BitmapImage(new Uri(this.BaseUri, "ms-appx:/Assets/dutchFlag/" + string.Format("{0:00}", ++dutchFlagCount) + ".gif"));
            System.Diagnostics.Debug.WriteLine(flagDutch.Source.GetValue(BitmapImage.UriSourceProperty));
            flagEnglish.Source = new BitmapImage(new Uri(this.BaseUri, "ms-appx:/Assets/englishFlag/" + string.Format("{0:00}", ++englishFlagCount) + ".gif"));
        }       

        private void startMapView(String language)
        {
            //TODO: do something with language
            //MapView map = new MapView();
            //mainControl.startMap(map);

            if (this.Frame != null)
            {

                this.Frame.Navigate(typeof(MapView));
                mainControl.startMap2();
            }
        }

        private void flagEnglish_Tapped(object sender, TappedRoutedEventArgs e)
        {
            timer.Stop();
            startMapView("english");
        }

        private void flagDutch_Tapped(object sender, TappedRoutedEventArgs e)
        {
            timer.Stop();
            startMapView("dutch");
        }
    }
}
