using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace HierInBreda.View
{
    public sealed partial class SightInfoFlyout : SettingsFlyout
    {
        public SightInfoFlyout()
        {
            this.InitializeComponent();
            SightImage.Source = new BitmapImage(new Uri("ms-appx:///" + "Assets/agslogo.jpg"));
        }

        public void updateSightInfo(String imageName,String desc,String name)
        {
            SightImage.Source = new BitmapImage(new Uri("ms-appx:///" + "Images/" + imageName + ".jpg"));
            SightName.Text = name;
            SightInfo.Text = desc;
        }
    }
}
