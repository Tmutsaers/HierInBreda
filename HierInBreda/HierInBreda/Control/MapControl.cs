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
     * @author:
     * @version:
     * @description:
     */
    public class MapControl
    {
        private MapView MapView;
        private List<Sight> Sights = new List<Sight>();
        //public Route route;
        private LocationRect userRadius;
        public Bing.Maps.Directions.Route Route { get; set; }
        private bool insideRoute = true;

        public MapControl()
        {
            MapView = new MapView(this);
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
            UpdateUserRadius("1", l);
            MapShapeLayer layer = new MapShapeLayer();
            if(userRadius.Intersects(Route.Bounds) && Route != null)
            {
                insideRoute = true;
            }
            else
                if(!userRadius.Intersects(Route.Bounds))
                {
                    insideRoute = false;
                }
        }

        void MapView_sightPinTapped(object sender, Pushpin pin)
        {
            
        }

        public Bing.Maps.Location getCurrentLocation()
        {
            return MapView.currentLoc;
        }
    }
}
