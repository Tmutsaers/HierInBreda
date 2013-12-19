using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierInBreda.Model
{
    /*
     * @author:Raymond Rohder
     * @version:1.0
     * @description: Class for notifications
     */
    class Notification
    {
        private string text;

        public Notification()
        {

        }

        public string getNotificationDutch(string notification)
        {
            switch (notification)
            {
                case "offRoute": text = "U bent van de route afgeweken";
                    break;
                case "batteryLow": text = "De batterijspanning is laag"; 
                    break;
                case "noGPS": text = "Er is geen bruikbaar GPSsignaal"; 
                    break;
                case "noInternet": text = "Er is geen internet verbinding"; 
                    break;
                case "warning": text = "";
                    break;
            }

            return text;
        }

        public string getNotificationEng(string notification)
        {
            switch (notification)
            {
                case "offRoute": text = "You deviated from route"; 
                    break;
                case "batteryLow": text = "Battery is low"; 
                    break;
                case "noGPS": text = "There is no valid GPS signal"; 
                    break;
                case "noInternet": text = "There is no internet connection"; 
                    break;
                case "warning": text = ""; 
                    break;
            }

            return text;
        }
    }
}
