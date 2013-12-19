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

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace HierInBreda.View
{
    public sealed partial class MapViewSettingsFlyout : SettingsFlyout
    {
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MapViewSettingsFlyout()
        {
            this.InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Start();
        }

        void timer_Tick(object sender, object e)
        {
            TimeText.Text = String.Format("Het is nu: " + string.Format("{0:00}", System.DateTime.Now.Hour) + ":" + string.Format("{0:00}", System.DateTime.Now.Minute) +":" + string.Format("{0:00}", System.DateTime.Now.Second));
        }      

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public void setSights(List<Sight> sights)
        {
            defaultViewModel["Sights"] = sights;
        }

        private void LanguageSelectButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainControl.promptUserForLanguage(this);
        }
    }
}
