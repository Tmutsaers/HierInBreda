using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Maps;
using HierInBreda.Model;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HierInBreda.Control
{
    /*
     * @author:Tim Mutsaers
     * @version: 1.1.0
     * @description:De klasse die de View Controleert 
     */
    public class MapControl
    {
        private MapView mapView;
        private List<Sight> Sights = new List<Sight>();
        //public Route route;
        private LocationRect userRadius;
        public Bing.Maps.Directions.Route Route { get; set; }
        private bool insideRoute = true;
        public DataControl dataControl;
        private List<Sight> sights;
        private Dictionary<Pushpin, Geofence> sightFences = new Dictionary<Pushpin, Geofence>();
        private Dictionary<Pushpin, Sight> sightpins = new Dictionary<Pushpin, Sight>();
        private Dictionary<Sight, Pushpin> pins = new Dictionary<Sight, Pushpin>();
        public bool insideGeofence { get; set; }

        public MapControl(DataControl dc, MapView mv)
        {
            dataControl = dc;
            mapView = mv;
            mapView.getInfoButton().IsEnabled = false;
            mapView.sightPinTapped += MapView_sightPinTapped;
            mapView.userPosChanged += MapView_userPosChanged;
            mapView.flyout.sightsListViewItemTapped += flyout_sightsListViewItemTapped;
            createSights();
        }

        void flyout_sightsListViewItemTapped(object source, Sight s)
        {
            Sight sight = s;
            if (sight.img != "")
            {
                if (sight.img.Length > 3)
                {
                    String[] images = sight.img.Split(',');
                    mapView.sightFlyout.updateSightInfo(images[0], sight.disc, sight.name);
                    mapView.flyout.Hide();
                    mapView.sightFlyout.Show();
                }
                else
                {
                    mapView.sightFlyout.updateSightInfo(sight.img, sight.disc, sight.name);
                    mapView.flyout.Hide();
                    mapView.sightFlyout.Show();
                }
            }
            else
            {
                mapView.sightFlyout.updateSightInfo(sight.img, sight.disc, sight.name);
                mapView.flyout.Hide();
                mapView.sightFlyout.Show();
            }
        }

        public async void createRoute()
        {
            List<Bing.Maps.Location> locs = new List<Bing.Maps.Location>();
            
                foreach (Sight s in sights)
                {
                    locs.Add(new Bing.Maps.Location(double.Parse(s.lat), double.Parse(s.longi)));
                }
                mapView.createRoute2(locs);
        }

        void Current_GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();
                foreach (GeofenceStateChangeReport report in reports)
                {
                    GeofenceState state = report.NewState;

                    Geofence geofence = report.Geofence;

                    if(state == GeofenceState.Exited)
                    {
                        mapView.getInfoButton().IsEnabled = false;
                    }

                    if (state == GeofenceState.Entered)
                    {
                        mapView.getInfoButton().IsEnabled = true;
                        foreach(Pushpin pin in sightpins.Keys)
                        {
                            if(sightpins[pin].Equals(geofence))
                            {
                                Sight sight = sightpins[pin];
                                if (sight.img != "")
                                {
                                    if (sight.img.Length > 3)
                                    {
                                        String[] images = sight.img.Split(',');
                                        mapView.sightFlyout.updateSightInfo(images[0], sight.disc, sight.name);
                                        mapView.getInfoButton().Icon = new SymbolIcon { Symbol = Symbol.Important };
                                    }
                                    else
                                    {
                                        mapView.sightFlyout.updateSightInfo(sight.img, sight.disc, sight.name);
                                        mapView.getInfoButton().Icon = new SymbolIcon { Symbol = Symbol.Important };
                                    }
                                }
                                else
                                {
                                    mapView.sightFlyout.updateSightInfo(sight.img, sight.disc, sight.name);
                                    mapView.getInfoButton().Icon = new SymbolIcon { Symbol = Symbol.Important };
                                }
                                foreach(object o in mapView.getMap().Children)
                                {
                                    if(o.GetType() == typeof(Pushpin))
                                    {
                                        Pushpin p = o as Pushpin;
                                        if(p.Equals(pin))
                                        {
                                            p.Background = new SolidColorBrush { Color = new Windows.UI.Color { A = 100, R = 100, B = 100, G = 100 } };
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
        }

        public async void createSights()
        {
            if (GeofenceMonitor.Current.Geofences.Count > 0)
                GeofenceMonitor.Current.Geofences.Clear();
            sights = await dataControl.getSight();
            //List<Bing.Maps.Location> locs = new List<Bing.Maps.Location>();
            foreach(Sight s in sights)
            {
                if(Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "nl")
                {
                    string[] sub = s.lat.Split('.');
                    s.lat = sub[0] + "," + sub[1];

                    sub = s.longi.Split('.');
                    s.longi = sub[0] + "," + sub[1];
                }

                Pushpin p = mapView.createSightPin(new Bing.Maps.Location(double.Parse(s.lat), double.Parse(s.longi)), s.name);
                pins.Add(s,p);
                sightpins.Add(p, s);
                Geofence fence  = mapView.createGeofence(new Bing.Maps.Location(double.Parse(s.lat), double.Parse(s.longi)), s.name);
                sightFences.Add(p,fence);
                GeofenceMonitor.Current.Geofences.Add(fence);
                //locs.Add(new Bing.Maps.Location(Double.Parse(s.lat), Double.Parse(s.longi)));
            }
            mapView.flyout.setSights(sights);
            //MapView.createSightPins(locs);
            createRoute();
            GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;
        }


        public void UpdateUserRadius(String Radius,Bing.Maps.Location loc)
        {
            var R = 6371;

            var radius = Radius;      //radius of the circle
            var latitude = loc.Latitude;    //latitude of the circle center
            var longitude = loc.Longitude;   //longitude of the circle center

            var lat = (latitude * Math.PI) / 180;
            var lon = (longitude * Math.PI) / 180;
            var d = float.Parse(radius) / R;
            var circlePoints = new LocationCollection();

            for (int x = 0; x <= 360; x += 5)
            {
                var p2 = new Bing.Maps.Location(0, 0);
                var brng = x * Math.PI / 180;
                p2.Latitude = Math.Asin(Math.Sin(lat) * Math.Cos(d) + Math.Cos(lat) * Math.Sin(d) * Math.Cos(brng));

                p2.Longitude = ((lon + Math.Atan2(Math.Sin(brng) * Math.Sin(d) * Math.Cos(lat),
                                 Math.Cos(d) - Math.Sin(lat) * Math.Sin(p2.Latitude))) * 180) / Math.PI;
                p2.Latitude = (p2.Latitude * 180) / Math.PI;
                circlePoints.Add(p2);
            }

            userRadius = new LocationRect(circlePoints);
        }

        async void MapView_userPosChanged(object sender, Bing.Maps.Location l)
        {
            UpdateUserRadius("0.1", l);
            MapShapeLayer layer = new MapShapeLayer();
            if (Route != null)
            {
                if (userRadius.Intersects(Route.Bounds) && Route != null)
                {
                    insideRoute = true;
                }
                else
                    if (!userRadius.Intersects(Route.Bounds))
                    {
                        mapView.popup = new Windows.UI.Popups.MessageDialog("U wijkt van de Route af", "Route");
                        await mapView.popup.ShowAsync();
                        insideRoute = false;
                    }
            }
        }

        void MapView_sightPinTapped(object sender, Pushpin pin)
        {
            Sight sight = sightpins[pin];
            if (sight.img != "")
            {
                if (sight.img.Length > 3)
                {
                    String[] images = sight.img.Split(',');
                    mapView.sightFlyout.updateSightInfo(images[0], sight.disc, sight.name);
                }
                else
                {
                    mapView.sightFlyout.updateSightInfo(sight.img, sight.disc, sight.name);
                }
            }
            else
            {
                mapView.sightFlyout.updateSightInfo(sight.img, sight.disc, sight.name);
            }
            mapView.sightFlyout.Show();
        }

        public Bing.Maps.Location getCurrentLocation()
        {
            return mapView.currentLoc;
        }
    }
}
