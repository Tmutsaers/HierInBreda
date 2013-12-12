using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Bing.Maps;
using HierInBreda.View;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HierInBreda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapView : Page
    {
        private Geolocator _geolocator;
        public Location currentLoc { get; set; }
        private Pushpin userPin;
        public MapViewSettingsFlyout flyout;

        public MapView()
        {
            this.InitializeComponent();
            zoomToLocation();
            flyout = new MapViewSettingsFlyout();
            flyout.Show();
        }

        //Zooms to the Users Location and makes a User PushPin
        private async void zoomToLocation()
        {
            _geolocator = new Geolocator();
            _geolocator.PositionChanged += _geolocator_PositionChanged;
            Geoposition currentPos = await _geolocator.GetGeopositionAsync();
            currentLoc = new Location(currentPos.Coordinate.Latitude, currentPos.Coordinate.Longitude);
            userPin = new Pushpin();
            userPin.Text = "Jij";
            MapLayer.SetPosition(userPin, currentLoc);
            Map.Children.Add(userPin);
            Map.SetView(currentLoc, 15.0);
        }


        //Automatically changes UserPushPin when the user's location changes
        async void _geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    currentLoc = new Location(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);
                    MapLayer.SetPosition(userPin, currentLoc);
                });
            }
            catch(Exception x)
            {
                System.Diagnostics.Debug.WriteLine(x);
            }
        }
    }
}
