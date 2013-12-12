using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace HierInBreda.Model
{
    /*
     * @author: Johannes [imp] Michel
     * @version: 
     * @description: 
     */
    class GPS
    {
        private Geolocator geolocator = null;

        ///<summary>
        /// Standard constructor 
        ///</summary>
        public GPS()
        {
            geolocator = new Geolocator();
        }


    }
}
