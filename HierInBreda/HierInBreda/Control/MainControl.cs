using HierInBreda.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
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

        StartControl startControl;
        DataControl dataControl;
        MapControl mapControl;

        private static MapView mapView;

        public MainControl()
        {

            dataControl = new DataControl();
            startControl = new StartControl();
            LanguageControl.GetInstance();
        }

        private static void startTutorial(IUICommand command)
        {
            if(mapView.Frame != null)
            {
                mapView.Frame.Navigate(typeof(View.TutorialView));
            }
        }

        public DataControl getDataControl()
        {
            return dataControl;
        }

        private static void skipTutorial(IUICommand command)
        {
        }

        public static async void promptUserForTutorial(MapView mapView)
        {
            ResourceLoader rl = new ResourceLoader();
            MainControl.mapView = mapView;
            MessageDialog msgDialog = new MessageDialog(rl.GetString("TutorialPromptText"), rl.GetString("TutorialButton.Label"));

            //OK Button
            UICommand okButton = new UICommand(rl.GetString("OkText"));
            okButton.Invoked = startTutorial;
            msgDialog.Commands.Add(okButton);

            //Skip Button
            UICommand skipButton = new UICommand(rl.GetString("CancelText"));
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

        public void startMap(MapView map)
        {
            mapControl = new MapControl(dataControl, map);
        }

        public void startMap2()
        {
            MapView.getInstance().dc = dataControl;
            MapView.getInstance().initMapView();
        }


        public static async void promptUserForLanguage(MapViewSettingsFlyout flyout)
        {
            MessageDialog msgDialog = new MessageDialog("Welke taal wilt u? What language do you want?", "Taal / Language");

            //NL Button
            UICommand nlButton = new UICommand("NL");
            nlButton.Invoked = switchToDutch;
            msgDialog.Commands.Add(nlButton);

            //EN Button
            UICommand enButton = new UICommand("EN");
            enButton.Invoked = switchToEnglish;
            msgDialog.Commands.Add(enButton);

            //Show message
            await msgDialog.ShowAsync();
        }

        private static void switchToEnglish(IUICommand command)
        {
            LanguageControl.GetInstance().lang = "en";
            MapView.getInstance().refresh();
        }

        private static void switchToDutch(IUICommand command)
        {
            LanguageControl.GetInstance().lang = "nl";
            MapView.getInstance().refresh();
        }
    }
}
