using HierInBreda.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierInBreda.Control
{
    /*
     * @author: Corné van Barneveld
     * @version: 1.0
     * @description: Communication between StartMenu, MainControl, Language en Tutorial
     */
    public class StartControl
    {
        private StartMenu startMenu;

        public StartControl()
        {
            startMenu = new StartMenu(this);
        }

        public StartMenu getStartMenu()
        {
            return startMenu;
        }
    }
}
