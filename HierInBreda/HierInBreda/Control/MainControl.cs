using HierInBreda.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace HierInBreda.Control
{
    /*
     * @author: Tektaara
     * @version: v1.0
     * @description: Control class for Main
     */
    class MainControl
    {
        private static MapView mapView;
        public MainControl()
        {
            LanguageControl.GetInstance();
        }

        private static void startTutorial(IUICommand command)
        {
            if(mapView.Frame != null)
            {
                mapView.Frame.Navigate(typeof(View.TutorialView));
            }
        }

        private static void skipTutorial(IUICommand command)
        {
        }

        public static async void promptUserForTutorial(MapView mapView)
        {
            MainControl.mapView = mapView;
            MessageDialog msgDialog = new MessageDialog("Would you like to follow the tutorial?", "Tutorial");

            //OK Button
            UICommand okButton = new UICommand("OK");
            okButton.Invoked = startTutorial;
            msgDialog.Commands.Add(okButton);

            //Skip Button
            UICommand skipButton = new UICommand("Skip");
            skipButton.Invoked = skipTutorial;
            msgDialog.Commands.Add(skipButton);

            //Show message
            await msgDialog.ShowAsync();
        }

        public static async void showMessage(string msg)
        {
            var msgDlg = new Windows.UI.Popups.MessageDialog(msg);
            msgDlg.DefaultCommandIndex = 1;
            await msgDlg.ShowAsync();
        }
    }
}
