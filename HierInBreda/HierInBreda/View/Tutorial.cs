﻿using System;
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
        public Tutorial(int i)
        {
            getTutorialTekst(i);
        }

        //private Image getImage()
        //{
        //    Image img = "";
        //    switch (i)
        //    {
        //        case 1: text = "";
        //            break;
        //        case 2: text = "";
        //            break;
        //        case 3: text = "";
        //            break;
        //        case 4: text = "";
        //            break;
        //        case 5: text = "";
        //            break;
        //        case 6: text = "";
        //            break;
        //    }
        //    return img;
        //}

        private string getTutorialTekst(int i)
        {
            string text = "";
            switch(i)
            {
                case 1: text = "uitleg";
                    break;
                case 2: text = "uitleg";
                    break;
                case 3: text = "uitleg";
                    break;
                case 4: text = "uitleg";
                    break;
                case 5: text = "uitleg";
                    break;
                case 6: text = "uitleg";
                    break;
            }
            return text;
        }
    }
}