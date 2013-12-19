using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Maps;
using HierInBreda.Model;
using Windows.Devices.Geolocation.Geofencing;

namespace HierInBreda.Control
{
    /*
     * @author:Tim Mutsaers
     * @version: 1.1.0
     * @description:De klasse die de View Controleert 
     */
    public class MapControl
    {
        private MapView MapView;
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

        public MapControl()
        {
            MapView = new MapView(this);
            dataControl = new DataControl();
            MapView.sightPinTapped += MapView_sightPinTapped;
            MapView.userPosChanged += MapView_userPosChanged;
            GeofenceMonitor.Current.GeofenceStateChanged += Current_GeofenceStateChanged;
        }

        void Current_GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();
                foreach (GeofenceStateChangeReport report in reports)
                {
                    GeofenceState state = report.NewState;

                    Geofence geofence = report.Geofence;

                    if (state == GeofenceState.Entered)
                    {
                        
                    }
                }
        }

        public async void createSightsDutch()
        {
            sights = await dataControl.getSightDutch();
            //List<Bing.Maps.Location> locs = new List<Bing.Maps.Location>();
            foreach(Sight s in sights)
            {
                Pushpin p = MapView.createSightPin(new Bing.Maps.Location(double.Parse(s.lat), double.Parse(s.longi)), s.name);
                pins.Add(s,p);
                sightpins.Add(p, s);
                sightFences.Add(p,MapView.createGeofence(new Bing.Maps.Location(Double.Parse(s.lat), Double.Parse(s.longi)), s.name));
                //locs.Add(new Bing.Maps.Location(Double.Parse(s.lat), Double.Parse(s.longi)));
            }
            //MapView.createSightPins(locs);
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

        void MapView_userPosChanged(object sender, Bing.Maps.Location l)
        {
            UpdateUserRadius("0.1", l);
            MapShapeLayer layer = new MapShapeLayer();
            //if(userRadius.Intersects(Route.Bounds) && Route != null)
            //{
            //    insideRoute = true;
            //}
            //else
            //    if(!userRadius.Intersects(Route.Bounds))
            //    {
            //        insideRoute = false;
            //    }
        }

        void MapView_sightPinTapped(object sender, Pushpin pin)
        {
            Sight s = sightpins[pin];
            MapView.sightFlyout.updateSightInfo(s.img, s.disc, s.name);
            MapView.sightFlyout.Show();
        }

        public Bing.Maps.Location getCurrentLocation()
        {
            return MapView.currentLoc;
        }
    }
}
