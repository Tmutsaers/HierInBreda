using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace HierInBreda.View
{
    public sealed partial class TutorialViewFlyout : SettingsFlyout
    {
        private int page = 0;
        private const int PAGE_MAX = 6;
        private const int FIRST_PAGE_NUM = 1;

        public TutorialViewFlyout()
        {
            this.InitializeComponent();
            page = FIRST_PAGE_NUM;
            Previous.IsEnabled = false;
            setTextAndImage();
        }

        private void Next_Tapped(object sender, TappedRoutedEventArgs e)
        {
            page++;
            if (page == PAGE_MAX)
                Next.IsEnabled = false;
            else
            {
                Next.IsEnabled = true;
                Previous.IsEnabled = true;
            }
            PageNumber.Text = "" + page + " / " + PAGE_MAX;
            setTextAndImage();
        }

        private void Previous_Tapped(object sender, TappedRoutedEventArgs e)
        {
            page--;
            if (page == FIRST_PAGE_NUM)
                Previous.IsEnabled = false;
            else
            {
                Next.IsEnabled = true;
                Previous.IsEnabled = true;
            }
            PageNumber.Text = "" + page + " / " + PAGE_MAX;
            setTextAndImage();
        }


        private void setTextAndImage()
        {
            ResourceLoader rl = new ResourceLoader();
            tutorialText.Text = rl.GetString("TutorialPage" + page);
            String languageFolder = "nl/";
            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "en")
                languageFolder = "en/";
            tutorialImage.Source = new BitmapImage(new Uri(this.BaseUri, "ms-appx:/Assets/TutorialImages/" + languageFolder + "page"+ page + ".png"));
        }
    }
}
