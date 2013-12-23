using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace HierInBreda.Model
{
    /*
     * @author: Raymond Rohder
     * @version: 1.0
     * @description: class wich contains all the information for the turorials
     */
    public class Tutorial
    {
        private static Tutorial instance;
        private const int maxTexts = 9; //INVULLEN DEZE!!

        public static Tutorial getInstance()
        {
            return instance != null ? instance : (instance = new Tutorial());
        }
        private Tutorial()
        {
            
        }

        public string getText(int i)
        {
            string text = "";
            if (Control.LanguageControl.GetInstance().getActiveLanguage().Name.CompareTo("Dutch") == 0)
            {
                switch (i)
                {
                    default:
                        break;
                }
            }

            if (Control.LanguageControl.GetInstance().getActiveLanguage().Name.CompareTo("English") == 0)
            {
                switch (i)
                {
                    default:
                        break;
                }
            }

            return "default";
        }
        public int getMax()
        {
            return maxTexts;
        }
    }
}
