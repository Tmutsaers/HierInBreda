﻿using HierInBreda.Control;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HierInBreda.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartMenu : Page
    {
        private StartControl startControl;
        private int dutchFlagCount;
        private int englishFlagCount;

        public StartMenu(StartControl startControl)
        {
            this.InitializeComponent();
            this.startControl = startControl;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Start();
        }

        void timer_Tick(object sender, object e)
        {
            dutchFlagCount++;
            englishFlagCount++;
            if (dutchFlagCount >= 87)
                dutchFlagCount = 0;
            if (englishFlagCount >= 37)
                englishFlagCount = 0;
            flagDutch.Source = new BitmapImage(new Uri(this.BaseUri, @"Assets/dutchFlag/" + string.Format("{0:00}", ++dutchFlagCount) + ".gif"));
            flagEnglish.Source = new BitmapImage(new Uri(this.BaseUri, @"Assets/englishFlag/" + string.Format("{0:00}", ++englishFlagCount) + ".gif"));
        }

        private void flagDutch_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //language = dutch
        }

        private void flagEnglish_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //language = english
        }      
    }
}
