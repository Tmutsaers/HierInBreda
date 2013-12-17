using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierInBreda.Model
{
    /*
     * @author: Johannes [imp] Michel
     * @version: 1.0
     * @description: Class containing the GPS of a location
     */
    public class Location
    {
        public double latitude { get; set; }
        public double longitude { get; set; }

        ///<summary>
        /// Standard constructor.
        ///</summary>
        public Location()
        {

        }
    }
}
