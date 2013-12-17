using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierInBreda.Control
{
    /*
     * @author:
     * @version:
     * @description:
     */
    class MainControl
    {
        StartControl startControl;

        public MainControl()
        {
            startControl = new StartControl();
            LanguageControl.GetInstance();
        }
    }
}
