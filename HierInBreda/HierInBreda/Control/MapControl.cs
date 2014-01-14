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
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;

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
        public List<Sight> sights { get; set; }
        private Dictionary<Pushpin, Geofence> sightFences = new Dictionary<Pushpin, Geofence>();
        private Dictionary<Pushpin, Sight> sightpins = new Dictionary<Pushpin, Sight>();
        private Dictionary<Sight, Pushpin> pins = new Dictionary<Sight, Pushpin>();
        public bool insideGeofence { get; set; }
        public bool showWarn = false;
        public LocationCollection pathLocations { get; set; }
        public Bing.Maps.Location userLoc { get; set; }

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
                        if(checkIfInGeofence() == false)
                        mapView.setInfoIcon(false);
                    }

                    if (state == GeofenceState.Entered)
                    {

                        mapView.setInfoIcon(true);
                        foreach(Pushpin pin in sightpins.Keys)
                        {
                            if(sightpins[pin].name == geofence.Id)
                            {
                                Sight sight = sightpins[pin];
                                
                                
                                String description = sight.disc;
                                if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "en")
                                    description = sight.discEng;
                                
                                if (sight.img != "")
                                {
                                    if (sight.img.Length > 3)
                                    {
                                        String[] images = sight.img.Split(',');
                                        mapView.sightFlyout.updateSightInfo(images[0], description, sight.name);
                                        mapView.setInfo();
                                    }
                                    else
                                    {
                                        mapView.sightFlyout.updateSightInfo(sight.img, description, sight.name);
                                        mapView.setInfo();
                                    }
                                }
                                else
                                {
                                    mapView.sightFlyout.updateSightInfo(sight.img, description, sight.name);
                                    mapView.setInfo();
                                }
                                foreach(object o in mapView.getMap().Children)
                                {
                                    if(o.GetType() == typeof(Pushpin))
                                    {
                                        Pushpin p = o as Pushpin;
                                        if(p.Equals(pin))
                                        {
                                            mapView.setPinVisited(p);
                                        }
                                    }

                                }
                            }
                        }
                        
                    }
                }
        }

        public bool checkIfInGeofence()
        {
            foreach(Sight s in sights)
            {
                if(inRadius(new Bing.Maps.Location(double.Parse(s.lat),double.Parse(s.longi)),userLoc,0.01))
                {
                    return true;
                }
            }
            return false;
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
            //drawRouteBounds();
            //drawRect();
            GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;
           // mapView.flyout.getDistance(sights);
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
            var u = 0.001;
            userRadius = new LocationRect(new LocationCollection{ loc, new Bing.Maps.Location(loc.Latitude+u,loc.Longitude),new Bing.Maps.Location(loc.Latitude,loc.Longitude+u),
            new Bing.Maps.Location(loc.Latitude+u,loc.Longitude+u), new Bing.Maps.Location(loc.Latitude-u,loc.Longitude), new Bing.Maps.Location(loc.Latitude,loc.Longitude-u),
            new Bing.Maps.Location(loc.Latitude-u,loc.Longitude-u)});
        }

        //public void drawRouteBounds()
        //{
        //    foreach(Sight s in sights)
        //    {
        //        if(!s.Equals(sights[sights.Count-1]))
        //        {
        //            Sight s2 = sights[sights.IndexOf(s)+1];
        //            LocationRect rect = new LocationRect(new LocationCollection{ new Bing.Maps.Location(double.Parse(s.lat),double.Parse(s.longi)),
        //                new Bing.Maps.Location(double.Parse(s2.lat),double.Parse(s2.longi))});
        //            routeBounds.Add(rect);
        //        }
        //    }
        //}

        //public void drawRect()
        //{
        //    foreach(LocationRect rect in routeBounds)
        //    {
        //        MapShapeLayer layer = new MapShapeLayer();
        //        MapPolyline line = new MapPolyline { Locations = new LocationCollection { rect.Northwest, rect.Southeast } };
        //        layer.Shapes.Add(line);
        //        mapView.getMap().ShapeLayers.Add(layer);
        //    }
        //}

        public Boolean inRadius(Bing.Maps.Location loc,Bing.Maps.Location curLoc, double radius)
        {
            if(getDistanceFromLatLonInKm(loc.Latitude,loc.Longitude,curLoc.Latitude,curLoc.Longitude) <= radius)
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

        async void MapView_userPosChanged(object sender, Bing.Maps.Location l)
        {
            UpdateUserRadius("0.1", l);
            userLoc = l;
            MapShapeLayer layer = new MapShapeLayer();
            if (pathLocations != null)
            {
                foreach (Bing.Maps.Location loc in pathLocations)
                {
                    if (inRadius(loc, l, 0.05))
                    {
                        insideRoute = true;
                        showWarn = false;
                        break;
                    }
                    else
                        if (!inRadius(loc, l, 0.05))
                        {

                            insideRoute = false;
                        }

                }
            }
            if(insideRoute == false && showWarn == false)
            {
                ResourceLoader rl = new ResourceLoader();
                mapView.popup = new Windows.UI.Popups.MessageDialog(rl.GetString("OffRoutePopup"), "Route");
                mapView.popup.Commands.Add(new UICommand(rl.GetString("Ok"), new UICommandInvokedHandler(this.CommandInvokedHandler)));
                showWarn = true;
                await mapView.popup.ShowAsync();
            }           
            
            //if (Route != null)
            //{
            //    if (userRadius.Intersects(Route.Bounds) && Route != null)
            //    {
            //        insideRoute = true;
            //    }
            //    else
            //        if (!userRadius.Intersects(Route.Bounds))
            //        {
            //            ResourceLoader rl = new ResourceLoader();
            //            mapView.popup = new Windows.UI.Popups.MessageDialog(rl.GetString("OffRoutePopup"), "Route");

            //            await mapView.popup.ShowAsync();
            //            insideRoute = false;
            //        }
            //}
        }

        private void CommandInvokedHandler(IUICommand command)
        {
        }

        void MapView_sightPinTapped(object sender, Pushpin pin)
        {
            Sight sight = sightpins[pin];
            String description = sight.disc;
            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "en")
                description = sight.discEng;

            if (sight.img != "")
            {
                if (sight.img.Length > 3)
                {
                    String[] images = sight.img.Split(',');
                    mapView.sightFlyout.updateSightInfo(images[0], description, sight.name);
                }
                else
                {
                    mapView.sightFlyout.updateSightInfo(sight.img, description, sight.name);
                }
            }
            else
            {
                mapView.sightFlyout.updateSightInfo(sight.img, description, sight.name);
            }
            mapView.sightFlyout.Show();
        }

        public Bing.Maps.Location getCurrentLocation()
        {
            return mapView.currentLoc;
        }
    }
}
