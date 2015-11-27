using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IslamicHadithWIN
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            DataBase();
        }

        async void DataBase()
        {
            try
            {
                // Database file name
               string fileName = "hadeeth.sqlite";

                // build the local path where we expect the database to be. if the db does not exist there yet, copy it from the package 
                string localPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, fileName);

                if (!File.Exists(localPath))
                {
                    StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/hadeeth.sqlite"));
                    await file.CopyAsync(ApplicationData.Current.LocalFolder, "hadeeth.sqlite");
                }
            }
            catch (Exception ex)
            {
                var cd = new ContentDialog();
                cd.Title = ex.ToString();
                cd.PrimaryButtonText = "تم";
                await cd.ShowAsync();
            }
        }

        private void barbtnSearchByBook_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchByBook));
        }

        private void barbtnSearchByAuthor_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchByAuthor));
        }

        private void barbtnSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void barbtnHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void barbtnAbout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void barbtnHadithSubject_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HadithSubject));
        }

        private void barbtnHadithTitle_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HadithTitle));
        }

        private void btnSahihMoslem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SahihMoslem));
        }

        private void btnSahihElBokhary_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SahihElBokhary));
        }

        private void btnSonanElTormozy_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SonanElTormozy));
        }

        private void btnSonanElNsaay_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SonanElNesaay));
        }

        private void btnSonanAbyDawod_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SonanAbyDawod));
        }

        private void btnSonanAbnMagh_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SonanAbnMagah));
        }

        private void btnMasnadAhmed_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MasnadAhmed));
        }

        private void btnMawteaMalek_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MawteaMalek));
        }

        private void btnSonanElDarmy_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SonanElDarmy));
        }
    }
}
