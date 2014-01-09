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
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace HierInBreda.View
{
    public delegate void SightsListViewItemTappedHandler(object source, Sight s);

    public sealed partial class MapViewSettingsFlyout : SettingsFlyout
    {
        private HierInBreda.Common.ObservableDictionary defaultViewModel = new HierInBreda.Common.ObservableDictionary();
        MapView mapView;
        private List<Sight> sights = new List<Sight>();
        public bool zoom;
        public event SightsListViewItemTappedHandler sightsListViewItemTapped;
        

        public MapViewSettingsFlyout(MapView mapView)
        {
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = LanguageControl.GetInstance().lang;
            this.InitializeComponent();
            this.mapView = mapView;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Start();
            this.Loaded += MapViewSettingsFlyout_Loaded;
        }

        protected void OnSightsListViewItemTapped(object o, Sight s)
        {
            Sight p = s;
            if (s != null && sightsListViewItemTapped != null)
            {
                sightsListViewItemTapped(this, p);
            }
        }

        void MapViewSettingsFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            SightsList.ItemsSource = sights;
            DefaultViewModel["Sights"] = sights;
        }

        void timer_Tick(object sender, object e)
        {
            ResourceLoader rl = new ResourceLoader();

            TimeText.Text = String.Format(rl.GetString("TimeText") + string.Format("{0:00}", System.DateTime.Now.Hour) + ":" + string.Format("{0:00}", System.DateTime.Now.Minute) +":" + string.Format("{0:00}", System.DateTime.Now.Second));
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
            //if (mapView.Frame != null)
            //{
            //    mapView.Frame.Navigate(typeof(View.TutorialView));
            //    this.Hide();
            //}
        }

        public bool isLegendaVisable()
        {
            return false;
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                mapView.setVisibilityLegenda(toggleSwitch.IsOn);                
            }
        }

        private void SightsList_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach(Sight s in sights)
            {
                if (e.OriginalSource.GetType() == typeof(ListViewItemPresenter))
                {
                    ListViewItemPresenter presenter = e.OriginalSource as ListViewItemPresenter;
                    Sight sp = presenter.Content as Sight;
                    if (s.Equals(sp))
                    {
                        OnSightsListViewItemTapped(this, s);
                    }
                }
                if(e.OriginalSource.GetType() == typeof(TextBlock))
                {
                    TextBlock tb = e.OriginalSource as TextBlock;
                    if(s.name == tb.Text)
                    {
                        OnSightsListViewItemTapped(this, s);
                    }
                }
            }
        }

        private async void ConnectionButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ResourceLoader rl = new ResourceLoader();
            MessageDialog mes = new MessageDialog(rl.GetString("unimplemented"));
            mes.Commands.Add(new UICommand(rl.GetString("Ok"), new UICommandInvokedHandler(this.CommandInvokedHandler)));
            await mes.ShowAsync();
        }

        private async void BitmapIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ResourceLoader rl = new ResourceLoader();
            MessageDialog mes = new MessageDialog(rl.GetString("unimplemented"));
            mes.Commands.Add(new UICommand(rl.GetString("Ok"), new UICommandInvokedHandler(this.CommandInvokedHandler)));
            await mes.ShowAsync();
        }

        private async void SymbolIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //ResourceLoader rl = new ResourceLoader();
            //MessageDialog mes = new MessageDialog(rl.GetString("unimplemented"));
            //mes.Commands.Add(new UICommand(rl.GetString("Ok"), new UICommandInvokedHandler(this.CommandInvokedHandler)));
            //await mes.ShowAsync();
            
            mapView.zoomToLocation2(mapView.currentLoc);
        }

        private void CommandInvokedHandler(IUICommand command)
        { 
        }
    }
}
