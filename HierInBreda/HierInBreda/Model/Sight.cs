using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HierInBreda.Model
{
    /*
     * @author: Raymond Rohder
     * @version: 1.0
     * @description: class containing all the information for the sights
     */
    public class Sight
    {
        public int id { get; set; }
        public string name { get; set; }
        public string lat { get; set; }
        public string longi { get; set; }
        public string img { get; set; }
        public string audio { get; set; }
        public string disc { get; set; }
        public string discEng { get; set; }

        public Sight()
        {
            
        }

    }
}
