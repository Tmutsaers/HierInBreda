using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace HierInBreda.Control
{
    /*
     * @author:
     * @version:
     * @description:
     */
    class MainControl
    {
        public MainControl()
        {
            LanguageControl.GetInstance();
            //showMessage("hello");
        }

        public static async void showMessage(string msg)
        {
            var msgDlg = new Windows.UI.Popups.MessageDialog(msg);
            msgDlg.DefaultCommandIndex = 1;
            await msgDlg.ShowAsync();
        }
    }
}
