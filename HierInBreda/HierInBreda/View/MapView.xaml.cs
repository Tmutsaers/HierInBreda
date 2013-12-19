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
using Windows.Devices.Geolocation.Geofencing;
using HierInBreda.Control;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HierInBreda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public delegate void SightPinTappedHandler(object sender,Pushpin pin);
    public delegate void UserPositionChangedHandler(object sender,Location l);

    public sealed partial class MapView : Page
    {
        private static MapView instance;
        public event SightPinTappedHandler sightPinTapped;
        public event UserPositionChangedHandler userPosChanged;
        private Geolocator _geolocator;
        public Location currentLoc { get; set; }
        private Pushpin userPin;
        public MapViewSettingsFlyout flyout { get; set; }
        public MapControl control;
        public SightInfoFlyout sightFlyout { get; set; }
        public MessageDialog popup { get; set; }
        public DataControl dc { get; set; }
        public MapControl mc { get; set; }

        public static MapView getInstance()
        {
            return instance != null ? instance : (instance = new MapView());
        }

        public MapView()
        {
            this.InitializeComponent();
            instance = this;
            zoomToLocation();
            flyout = new MapViewSettingsFlyout(this);
            sightFlyout = new SightInfoFlyout();
            //flyout.Show();
            Uri uri = new Uri("ms-appx:///" + "Assets/agslogo.jpg");
            AgsLogo.Source = new BitmapImage(uri);
            InfoButton.Icon = new SymbolIcon { Symbol = Symbol.Important };
            System.Diagnostics.Debug.WriteLine("Test");
        }

        public void initMapView()
        {
            mc = new MapControl(dc, this);
        }

        public MapView(MapControl control)
        {
            this.InitializeComponent();
            this.control = control;
            zoomToLocation();
            flyout = new MapViewSettingsFlyout(this);
            sightFlyout = new SightInfoFlyout();
            //flyout.Show();
            Uri uri = new Uri("ms-appx:///" + "Assets/agslogo.jpg");
            AgsLogo.Source = new BitmapImage(uri);
            InfoButton.Icon = new SymbolIcon { Symbol = Symbol.Important };
        }

        public AppBarButton getInfoButton()
        {
            return InfoButton;
        }

        public async void createRoute(List<Location> locs)
        {
            WaypointCollection col = new WaypointCollection();
            foreach(Location loc in locs)
            {
                col.Add(new Waypoint(loc));
            }

            DirectionsManager manager = Map.DirectionsManager;
            manager.Waypoints = col;

            RouteResponse route_response = await manager.CalculateDirectionsAsync();
            manager.ShowRoutePath(route_response.Routes[0]);
            control.Route = route_response.Routes[0];
        }

        public Pushpin createSightPin(Location l,String id)
        {
            Pushpin p = new Pushpin();
            p.Text = id;
            MapLayer.SetPosition(p, l);
            Map.Children.Add(p);
            p.Background = new SolidColorBrush { Color = new Windows.UI.Color { A = 100, R = 100, G = 100, B = 100 } };
            p.Tapped += p_Tapped;
            System.Diagnostics.Debug.WriteLine("Lat: " + l.Latitude);
            System.Diagnostics.Debug.WriteLine("Lon: " + l.Longitude);
            return p;
        }

        public List<Pushpin> createSightPins(List<Location> locations)
        {
            clearGeofences();
            List<Pushpin> pins = new List<Pushpin>();
            foreach(Location l in locations)
            {
                Pushpin p = new Pushpin();
                p.Text = "BZW";
                MapLayer.SetPosition(p, l);
                Map.Children.Add(p);
                pins.Add(p);
                p.Tapped += p_Tapped;
            }
            return pins;
        }

        void p_Tapped(object sender, TappedRoutedEventArgs e)
        {
            OnSightPinTappedHandler(this, sender as Pushpin);
        }

        protected void OnSightPinTappedHandler(object o,Pushpin p)
        {
            Pushpin pin = p;
            if (pin != null && sightPinTapped != null)
            {
                sightPinTapped(this, pin);
            }
        }

        protected void OnUserPositionChanged(object o,Location l)
        {
            Location loc = l;
            if(l != null && userPosChanged != null)
            {
                userPosChanged(this, l);
            }
        }

        public void clearGeofences()
        {
            GeofenceMonitor.Current.Geofences.Clear();
        }

        public Geofence createGeofence(Location l,String name)
        {
            Geofence fence = new Geofence(name, new Geocircle(new BasicGeoposition { Altitude = 0.0, Latitude = l.Latitude, Longitude = l.Longitude }, 0.2));
            return fence;
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
            _geolocator.DesiredAccuracy = PositionAccuracy.Default;
            _geolocator.PositionChanged += _geolocator_PositionChanged;
        }

        public void zoomToLocation2(Location l)
        {
            Map.SetView(l,15.0);
        }


        //Automatically changes UserPushPin when the user's location changes
        async void _geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (currentLoc != null && UserIsInRadius(0.1, new Location(args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Point.Position.Longitude), currentLoc))
                    {
                        currentLoc = new Location(args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Point.Position.Longitude);
                        System.Diagnostics.Debug.WriteLine("Latitude:  {0} \nLongitude: {1}", args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Point.Position.Longitude);
                        if (args.Position.Coordinate.Point.Position.Latitude > 0 && args.Position.Coordinate.Point.Position.Longitude > 0)
                        {
                            MapLayer.SetPosition(userPin, currentLoc);
                            zoomToLocation2(currentLoc);
                            OnUserPositionChanged(this, currentLoc);
                        }
                    }
                });
            }
            catch (Exception x)
            {
                System.Diagnostics.Debug.WriteLine(x);
            }
        }

        public bool UserIsInRadius(double radius,Location newLoc,Location oldLoc)
        {
            if(getDistanceFromLatLonInKm(newLoc.Latitude,newLoc.Longitude,oldLoc.Latitude,oldLoc.Longitude) <= radius)
            {
                return true;
            }
            return false;
        }

        public double getDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = deg2rad(lat2 - lat1);  // deg2rad below
            var dLon = deg2rad(lon2 - lon1);
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in km
            return d;
        }
        public double deg2rad(double deg)
        {
            return deg * (Math.PI / 180);
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

        private void TutorialButton_Click(object sender, RoutedEventArgs e)
        {
            MainControl.promptUserForTutorial(this);
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            InfoButton.Icon = new SymbolIcon { Symbol = Symbol.MapPin };
            sightFlyout.Show();
        }

    }
}
