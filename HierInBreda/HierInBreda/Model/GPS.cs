using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;

namespace HierInBreda.Model
{
    /*
     * @author: Johannes [imp] Michel
     * @version: 1.0
     * @description: Useless model class for a geolocator.
     */
    class GPS
    {
        private Geolocator geolocator = null;
        private double latitude;
        private double longitude;

        ///<summary>
        /// Standard constructor generating a Geolocator.
        ///</summary>
        public GPS()
        {
            geolocator = new Geolocator();
        }

        ///<summary>
        /// Method to call setGeolocation for the Longitude.
        ///</summary>
        public double GetLongitude()
        {
            setGeolocation();
            return latitude;
        }

        ///<summary>
        /// Method to call setGeolocation for the Longitude.
        ///</summary>
        public double GetLatitude()
        {
            setGeolocation();
            return longitude;
        }

        ///<summary>
        /// Asks the geolocator to locate the current position.
        ///</summary>
        async private void setGeolocation()
        {
            try
            {
                Geoposition pos = await geolocator.GetGeopositionAsync();

                latitude = pos.Coordinate.Latitude;
                longitude = pos.Coordinate.Longitude;
            }
            catch (System.UnauthorizedAccessException)
            {
                
            }
            catch (TaskCanceledException)
            {
                
            }
            catch (Exception)
            {
                
            }
            finally
            {
            }
        }
    }
}
