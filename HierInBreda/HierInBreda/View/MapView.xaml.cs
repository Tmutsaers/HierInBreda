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
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Bing.Maps.Directions;
using System.Threading;
using System.Threading.Tasks;

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

        private static Frame currentFrame;

        public static Frame GetFrame()
        {
            return currentFrame;
        }

        public MapView()
        {
            currentFrame = this.Frame;
            this.InitializeComponent();
            zoomToLocation();
            flyout = new MapViewSettingsFlyout();
            //flyout.Show();
            Uri uri = new Uri("ms-appx:///" + "Assets/agslogo.jpg");
            AgsLogo.Source = new BitmapImage(uri);
            InfoButton.Icon = new SymbolIcon { Symbol = Symbol.Important };
            Control.MainControl.promptUserForTutorial();
        }

        public async void createRoute(Location a,Location b)
        {
            WaypointCollection waypointCol = new WaypointCollection();
            waypointCol.Add(new Waypoint(a));
            waypointCol.Add(new Waypoint(b));

            DirectionsManager manager = Map.DirectionsManager;
            manager.Waypoints = waypointCol;

            RouteResponse route_response = await manager.CalculateDirectionsAsync();
            manager.ShowRoutePath(route_response.Routes[0]);
        }

        public List<Pushpin> createSightPins(List<Location> locations)
        {
            List<Pushpin> pins = new List<Pushpin>();
            foreach(Location l in locations)
            {
                Pushpin p = new Pushpin();
                p.Text = "BZW";
                MapLayer.SetPosition(p, l);
                Map.Children.Add(p);
                pins.Add(p);
            }
            return pins;
        }

        //Zooms to the Users Location and makes a User PushPin
        private async void zoomToLocation()
        {
            _geolocator = new Geolocator();
            Geoposition currentPos = await _geolocator.GetGeopositionAsync();
            currentLoc = new Location(currentPos.Coordinate.Latitude, currentPos.Coordinate.Longitude);
            userPin = new Pushpin();
            userPin.Text = "Jij";
            userPin.Background = new SolidColorBrush(new Windows.UI.Color { A = 100, B = 0, G = 100, R = 0 });
            MapLayer.SetPosition(userPin, currentLoc);
            Map.Children.Add(userPin);
            Map.SetView(currentLoc, 15.0);
            //_geolocator.MovementThreshold = 10;
            //_geolocator.ReportInterval = 500;
            _geolocator.DesiredAccuracy = PositionAccuracy.High;
            _geolocator.PositionChanged += _geolocator_PositionChanged;
        }


        //Automatically changes UserPushPin when the user's location changes
        async void _geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    currentLoc = new Location(args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Point.Position.Longitude);
                    System.Diagnostics.Debug.WriteLine("Latitude:  {0} \nLongitude: {1}", args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Point.Position.Longitude);
                    if (args.Position.Coordinate.Point.Position.Latitude > 0 && args.Position.Coordinate.Point.Position.Longitude > 0)
                        MapLayer.SetPosition(userPin, currentLoc);
                });
            }
            catch (Exception x)
            {
                System.Diagnostics.Debug.WriteLine(x);
            }
        }

        //private void geo_PositionChanged(Geolocator sender, PositionChangedEventArgs e)
        //{
        //    _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        Geoposition pos = (a.Context as IPositionChangedEventArgs).Position;
        //        textLatitude.Text = "Latitude: " + pos.Coordinate.Latitude.ToString();
        //        textLongitude.Text = "Longitude: " + pos.Coordinate.Longitude.ToString();
        //        textAccuracy.Text = "Accuracy: " + pos.Coordinate.Accuracy.ToString();
        //    });
        //}

        private void AppbarButton_Click(object sender, RoutedEventArgs e)
        {
            flyout.Show();
        }



    }
}
