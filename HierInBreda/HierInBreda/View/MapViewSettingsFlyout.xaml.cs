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
using Windows.UI.Xaml.Navigation;
using HierInBreda.Model;
using HierInBreda.Control;
using HierInBreda.Common;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace HierInBreda.View
{
    public sealed partial class MapViewSettingsFlyout : SettingsFlyout
    {
        private HierInBreda.Common.ObservableDictionary defaultViewModel = new HierInBreda.Common.ObservableDictionary();
        MapView mapView;
        private List<Sight> sights = new List<Sight>();
        

        public MapViewSettingsFlyout(MapView mapView)
        {
            this.InitializeComponent();
            this.mapView = mapView;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Start();
            this.Loaded += MapViewSettingsFlyout_Loaded;
        }

        void MapViewSettingsFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            SightsList.ItemsSource = sights;
            DefaultViewModel["Sights"] = sights;
        }

        void timer_Tick(object sender, object e)
        {
            TimeText.Text = String.Format("Het is nu: " + string.Format("{0:00}", System.DateTime.Now.Hour) + ":" + string.Format("{0:00}", System.DateTime.Now.Minute) +":" + string.Format("{0:00}", System.DateTime.Now.Second));
        }      

        public HierInBreda.Common.ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public void setSights(List<Sight> sights)
        {
            this.sights = sights;
           
        }


        private void LanguageSelectButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainControl.promptUserForLanguage(this);
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (mapView.Frame != null)
            {
                mapView.Frame.Navigate(typeof(View.TutorialView));
                this.Hide();
            }
        }
    }
}
