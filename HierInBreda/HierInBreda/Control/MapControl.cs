using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Maps;

namespace HierInBreda.Control
{
    /*
     * @author:
     * @version:
     * @discription:
     */
    class MapControl
    {
        public MapView MapView;

        public MapControl()
        {
            MapView = new MapView();
        }

        public Location getCurrentLocation()
        {
            return MapView.currentLoc;
        }
    }
}
