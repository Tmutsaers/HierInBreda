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
using System.Globalization;
using System.Resources;
using Windows.ApplicationModel.Resources;
using System.Runtime.Serialization.Json;
using BingMapsRESTService.Common.JSON;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HierInBreda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public delegate void SightPinTappedHandler(object sender,Pushpin pin);
    public delegate void UserPositionChangedHandler(object sender,Bing.Maps.Location l);

    public sealed partial class MapView : Page
    {
        private Response dresp;
        private static MapView instance;
        public event SightPinTappedHandler sightPinTapped;
        public event UserPositionChangedHandler userPosChanged;
        private Geolocator _geolocator;
        public Bing.Maps.Location currentLoc { get; set; }
        private Pushpin userPin;
        public MapViewSettingsFlyout flyout { get; set; }
        public MapControl control;
        public SightInfoFlyout sightFlyout { get; set; }
        public TutorialViewFlyout turorialViewFlyout { get; set; }
        public MessageDialog popup { get; set; }
        public MessageDialog InternetPopup { get; set; }
        public bool popupShown = false;
        public DataControl dc { get; set; }
        public MapControl mc { get; set; }
        public MapShapeLayer walkedPathLayer = new MapShapeLayer();
        public MapShapeLayer RouteLayer = new MapShapeLayer();
        private DispatcherTimer timer;
        public bool Finished = false;
        public bool zoomFinished = false;

        public static MapView getInstance()
        {
            return instance != null ? instance : (instance = new MapView());
        }

        public Map getMap()
        {
            return Map;
        }

        public MapView()
        {
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = LanguageControl.GetInstance().lang;
            this.InitializeComponent();
            instance = this;
            Map.ShapeLayers.Add(walkedPathLayer);
            zoomToLocation();
            flyout = new MapViewSettingsFlyout(this);
            sightFlyout = new SightInfoFlyout();
            turorialViewFlyout = new TutorialViewFlyout();
            //flyout.Show();
            Uri uri = new Uri("ms-appx:///" + "Assets/agslogo2.png");
            Uri uri2 = new Uri("ms-appx:///" + "Assets/VVVLogo.png");
            AgsLogo.Source = new BitmapImage(uri);
            VVVLogo.Source = new BitmapImage(uri2);
            InfoButton.Icon = new SymbolIcon { Symbol = Symbol.MapPin };
            //System.Diagnostics.Debug.WriteLine("Test");
        }

        public void initMapView()
        {
            mc = new MapControl(dc, this);
        }

        public AppBarButton getInfoButton()
        {
            return InfoButton;
        }

        public async void createRouteToVVV(Bing.Maps.Location curLoc,Bing.Maps.Location vvvLoc)
        {
            try
            {
                foreach (MapShapeLayer layer in Map.ShapeLayers)
                {
                    if (layer.Equals(RouteLayer))
                    {
                        layer.Shapes.Clear();
                        RouteLayer.Shapes.Clear();
                    }
                }
                Map.ShapeLayers.Clear();

                WaypointCollection routePoints = new WaypointCollection { new Waypoint(curLoc), new Waypoint(vvvLoc) };
                DirectionsManager manager = Map.DirectionsManager;
                manager.Waypoints.Clear();
                manager.RequestOptions.RouteMode = RouteModeOption.Walking;
                manager.Waypoints = routePoints;
                manager.RenderOptions.WalkingPolylineOptions.Visible = true;

                RouteResponse resp = await manager.CalculateDirectionsAsync();
                resp.Routes[0].RoutePath.LineWidth = 10.0;
                resp.Routes[0].RoutePath.LineColor = new Windows.UI.Color { A = 200, R = 200, B = 0, G = 2 };
                MapShapeLayer layer10 = new MapShapeLayer();
                
                

                LocationCollection locs = new LocationCollection();
                foreach(Bing.Maps.Location l in resp.Routes[0].RoutePath.PathPoints)
                {
                    Pushpin p = new Pushpin();
                    p.Text = "Pin";
                    MapLayer.SetPosition(p, l);
                    Map.Children.Add(p);
                    locs.Add(l);
                }
                MapPolyline line = new MapPolyline { Locations = locs };
                line.Color = new Windows.UI.Color { A = 100, G = 0, B = 200, R = 0 };
                line.Visible = true;
                line.Width = 10.0;
                layer10.Shapes.Add(line);
                Map.ShapeLayers.Add(layer10);

                manager.ShowRoutePath(resp.Routes[0]);
            }
            catch(Exception d)
            {
                System.Diagnostics.Debug.WriteLine(d);
                showInternetPopup();
            }
        }

        public async void setInfo()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                InfoButton.Icon = new SymbolIcon { Symbol = Symbol.Important };
            });
        }

        public async void setPinVisited(Pushpin p)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                p.Background = new SolidColorBrush { Color = new Windows.UI.Color { A = 100, R = 100, B = 100, G = 100 } };
            });
        }

        public async void setInfoIcon(bool bol)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    InfoButton.IsEnabled = bol;
                });
            }
            catch(Exception)
            {

            }
        }

        private async Task<Response> GetResponse(Uri uri)
        {
            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
            var response = await client.GetAsync(uri);
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                return ser.ReadObject(stream) as Response;
            }
        }

        public async Task<double> getTotalDistanceKM(List<Bing.Maps.Location> locs)
        {
            string yuri = "http://dev.virtualearth.net/REST/V1/Routes/Walking?o=json";
            int count = 0;
            foreach (Bing.Maps.Location l in locs)
            {
                if (count >= 25) break; //limited to 25 locs....
                double lat, longt;
                lat = l.Latitude;
                longt = l.Longitude;
                yuri += string.Format("&wp.{0}={1},{2}", count, lat.ToString(CultureInfo.InvariantCulture), longt.ToString(CultureInfo.InvariantCulture)); //not sure if lat and long need to switch. msdn is very helpful (not)
                count++;
            }
            //yuri += "&optmz=distance"; // uncomment if you need to optimise for anything.
            yuri += string.Format("&key={0}", Map.Credentials);
            Response r = await GetResponse(new Uri(yuri));
            if (r != null &&
                    r.ResourceSets != null &&
                    r.ResourceSets.Length > 0 &&
                    r.ResourceSets[0].Resources != null &&
                    r.ResourceSets[0].Resources.Length > 0)
            {
                BingMapsRESTService.Common.JSON.Route route = r.ResourceSets[0].Resources[0] as BingMapsRESTService.Common.JSON.Route;
                return route.TravelDistance;
            }
            else
            {
                return 42.0f;
            }
        }

        public async void createRoute2(List<Bing.Maps.Location> locs)
        {
            double km = await getTotalDistanceKM(locs);
            int breakpoint;
            try
            {
                LocationCollection routePoints = new LocationCollection();
                WaypointCollection col = new WaypointCollection();

                for (int i = 0; i < locs.Count / 2; i++)
                {
                    col.Add(new Waypoint(locs[i]));
                }

                DirectionsManager manager = Map.DirectionsManager;
                manager.RequestOptions.RouteMode = RouteModeOption.Walking;
                manager.Waypoints = col;
                manager.RenderOptions.WaypointPushpinOptions.Visible = false;

                RouteResponse resp = await manager.CalculateDirectionsAsync();
                foreach (Bing.Maps.Location l in resp.Routes[0].RoutePath.PathPoints)
                {
                    routePoints.Add(l);
                }

                manager.Waypoints.Clear();
                col.Clear();

                for (int i = locs.Count / 2; i < locs.Count; i++)
                {
                    col.Add(new Waypoint(locs[i]));
                }

                manager.Waypoints = col;

                resp = await manager.CalculateDirectionsAsync();
                foreach (Bing.Maps.Location l in resp.Routes[0].RoutePath.PathPoints)
                {
                    routePoints.Add(l);
                }


                MapPolyline line = new MapPolyline { Locations = routePoints };
                line.Color = new Windows.UI.Color { A = 50, R = 0, G = 0, B = 200 };
                line.Width = 10.0;

                RouteLayer.Shapes.Add(line);
                mc.pathLocations = routePoints;
                Map.ShapeLayers.Add(RouteLayer);
            }
            catch(Exception d)
            {
                System.Diagnostics.Debug.WriteLine(d);
                showInternetPopup();
            }
        }

        public async void createRoute(List<Bing.Maps.Location> locs)
        {
            WaypointCollection col = new WaypointCollection();
            for (int i = 0; i < locs.Count; i = i + 2)
            {
                col.Add(new Waypoint(locs[i]));
                //System.Diagnostics.Debug.WriteLine("Lat: " + locs[i].Latitude);
                //System.Diagnostics.Debug.WriteLine("Lon: " + locs[i].Longitude);
            }
            //col.Add(new Waypoint(locs[0]));
            //col.Add(new Waypoint(locs[locs.Count - 1]));
            //foreach (Location loc in locs)
            //{
            //    col.Add(new Waypoint(loc));
            //    System.Diagnostics.Debug.WriteLine("Lat: " + loc.Latitude);
            //    System.Diagnostics.Debug.WriteLine("Lon: " + loc.Longitude);
            //}

            DirectionsManager manager = Map.DirectionsManager;
            manager.Waypoints = col;
            manager.RequestOptions.RouteMode = RouteModeOption.Walking;

            RouteResponse route_response = await manager.CalculateDirectionsAsync();
            route_response.Routes[0].RoutePath.LineColor = new Windows.UI.Color { A = 200, R = 0, B = 200, G = 0 };
            route_response.Routes[0].RoutePath.LineWidth = 10.0;
            manager.ShowRoutePath(route_response.Routes[0]);
            mc.Route = route_response.Routes[0];
        }

        public Pushpin createSightPin(Bing.Maps.Location l,String id)
        {
            Pushpin p = new Pushpin();
            p.Text = id;
            MapLayer.SetPosition(p, l);
            Map.Children.Add(p);
            p.Background = new SolidColorBrush { Color = new Windows.UI.Color { A = 100, R = 0 ,G = 100, B = 0 } };
            p.Tapped += p_Tapped;
            //System.Diagnostics.Debug.WriteLine("Lat: " + l.Latitude);
            //System.Diagnostics.Debug.WriteLine("Lon: " + l.Longitude);
            return p;
        }

        public List<Pushpin> createSightPins(List<Bing.Maps.Location> locations)
        {
            clearGeofences();
            List<Pushpin> pins = new List<Pushpin>();
            foreach(Bing.Maps.Location l in locations)
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

        protected void OnUserPositionChanged(object o,Bing.Maps.Location l)
        {
            Bing.Maps.Location loc = l;
            if(l != null && userPosChanged != null)
            {
                userPosChanged(this, l);
            }
        }

        public void clearGeofences()
        {
            GeofenceMonitor.Current.Geofences.Clear();
        }

        public Geofence createGeofence(Bing.Maps.Location l,String name)
        {
            Geofence fence = new Geofence(name, new Geocircle(new BasicGeoposition { Altitude = 0.0, Latitude = l.Latitude, Longitude = l.Longitude }, 10));
            return fence;
        }

        //Zooms to the Users Location and makes a User PushPin
        private async void zoomToLocation()
        {
            try
            {
                _geolocator = null;
                _geolocator = new Geolocator();
                Geoposition currentPos = await _geolocator.GetGeopositionAsync();
                currentLoc = new Bing.Maps.Location(currentPos.Coordinate.Latitude, currentPos.Coordinate.Longitude);
                userPin = new Pushpin();
                ResourceLoader rl = new ResourceLoader();
                userPin.Text = rl.GetString("UserPinText");
                userPin.Background = new SolidColorBrush(new Windows.UI.Color { A = 100, B = 0, G = 0, R = 100 });
                MapLayer.SetPosition(userPin, currentLoc);
                Map.Children.Add(userPin);
                Map.SetView(currentLoc, 15.0);
                //_geolocator.MovementThreshold = 10;
                //_geolocator.ReportInterval = 500;
                _geolocator.DesiredAccuracy = PositionAccuracy.Default;
                _geolocator.PositionChanged += _geolocator_PositionChanged;
                timer = new DispatcherTimer();
                timer.Tick += timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 0, 0, 2000);
                timer.Start();
                if(Finished == false)
                {
                    Finished = true;
                    zoomFinished = true;
                    mc.createRoute();
                }
            }
            catch(Exception d)
            {
                System.Diagnostics.Debug.WriteLine(d);
                showInternetPopup();
            }
        }

        public void zoomToLocation2(Bing.Maps.Location l)
        {
            Map.SetView(l, Map.ZoomLevel);
            //Map.SetView(l,15.0);
        }


        //Automatically changes UserPushPin when the user's location changes
        async void _geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (currentLoc != null && UserIsInRadius(10, new Bing.Maps.Location(args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Point.Position.Longitude), currentLoc))
                    {
                        //System.Diagnostics.Debug.WriteLine("Latitude:  {0} \nLongitude: {1}", args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Point.Position.Longitude);
                        if (args.Position.Coordinate.Point.Position.Latitude > 0 && args.Position.Coordinate.Point.Position.Longitude > 0)
                        {
                            MapLayer.SetPosition(userPin, currentLoc);
                            drawMovedLine(currentLoc, new Bing.Maps.Location(args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Point.Position.Longitude));
                            //zoomToLocation2(currentLoc);
                            OnUserPositionChanged(this, currentLoc);
                            currentLoc = new Bing.Maps.Location(args.Position.Coordinate.Point.Position.Latitude, args.Position.Coordinate.Point.Position.Longitude);
                        }
                    }
                });
            }
            catch (Exception x)
            {
                System.Diagnostics.Debug.WriteLine(x);
                showInternetPopup();
            }
        }

        public void drawMovedLine(Bing.Maps.Location l1, Bing.Maps.Location l2)
        {
            foreach (MapShapeLayer layer in Map.ShapeLayers)
            {
                if (layer.Equals(walkedPathLayer))
                {
                    MapPolyline line = new MapPolyline { Locations = new LocationCollection { l1, l2 } };
                    line.Color = new Windows.UI.Color { A = 255, B = 0, G = 0, R = 100 };
                    line.Width = 10.0;
                    layer.Shapes.Add(line);
                }
            }

        }

        public bool UserIsInRadius(double radius,Bing.Maps.Location newLoc,Bing.Maps.Location oldLoc)
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
            turorialViewFlyout.Show();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
                InfoButton.Icon = new SymbolIcon { Symbol = Symbol.MapPin };
                sightFlyout.Show();
        }

        public void refresh()
        {
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = LanguageControl.GetInstance().lang;
            flyout = new MapViewSettingsFlyout(this);
            sightFlyout = new SightInfoFlyout();

            ResourceLoader rl = new ResourceLoader();
            AppbarButton.Label = rl.GetString("AppbarButtonLabel");
            InfoButton.Label = rl.GetString("InfoButtonLabel");
            TutorialButton.Label = rl.GetString("TutorialButtonLabel");
            legendaBezochteBZW.Text = rl.GetString("Legend11");
            legendaOnbezochteBZW.Text = rl.GetString("Legend21");
            legendaRoute.Text = rl.GetString("Legenda31");
            mc.createSights();
        }

        public void setVisibilityLegenda(Boolean vis)
        {
            if (vis)
                mainGridLegenda.Visibility = Visibility.Visible;
            else
                mainGridLegenda.Visibility = Visibility.Collapsed;

        }

        private async void VVVBackButton_Click(object sender, RoutedEventArgs e)
        {
            ResourceLoader rl = new ResourceLoader();
            MessageDialog mes = new MessageDialog(rl.GetString("VVVWarning"));
            mes.Commands.Add(new UICommand(rl.GetString("Ok"), new UICommandInvokedHandler(this.CommandInvokedHandler)));
            mes.Commands.Add(new UICommand(rl.GetString("Cancel"), new UICommandInvokedHandler(this.CommandInvokedHandler)));
            await mes.ShowAsync();

        }

        private async void CommandInvokedHandler(IUICommand command)
        {
            if(command.Label == "Ok")
            {
                createRouteToVVV(currentLoc, new Bing.Maps.Location(double.Parse(mc.sights[0].lat), double.Parse(mc.sights[0].longi)));
            }
            
        }

        async void timer_Tick(object sender, object e)
        {
            try
            {
                await _geolocator.GetGeopositionAsync();
                if(Finished == false)
                {
                    Finished = true;
                    mc.createRoute();
                }
            }
            catch(Exception d)
            {
                
                System.Diagnostics.Debug.WriteLine(d);
                showInternetPopup();
            }
        }

        public async void showInternetPopup()
        {
            try
            {
                if (popupShown == false)
                {
                    InternetPopup = new Windows.UI.Popups.MessageDialog("No internet Connection", "Internet");
                    InternetPopup.Commands.Add(new UICommand("Try Again", new UICommandInvokedHandler(this.CommandInvokedHandler2)));
                    popupShown = true;
                    await InternetPopup.ShowAsync();
                }
            }
            catch(Exception d)
            {

            }
        }

        private void CommandInvokedHandler2(IUICommand command)
        {
            // Display message showing the label of the command that was invoked
            popupShown = false;
            if (zoomFinished == false)
            {
                Finished = false;
                zoomToLocation();
            }
            else
            {
                Finished = false;
                timer_Tick(new Object(), new object());
            }
        }
    }
}
