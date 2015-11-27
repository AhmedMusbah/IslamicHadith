using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IslamicHadithWIN
{
    public sealed partial class HadithSubject : Page
    {
        public HadithSubject()
        {
            this.InitializeComponent();

            HadithSubjectContents();
        }

        public async System.Threading.Tasks.Task<ObservableCollection<Hadith>> HadithSubjectContents()
        {
            listSubject.Visibility = Visibility.Visible;
            listHadith.Visibility = Visibility.Collapsed;

            ObservableCollection<Hadith> list = new ObservableCollection<Hadith>();

            try
            {
                using (var connection = new SQLiteConnection("hadeeth.sqlite"))
                {
                    using (var statement = connection.Prepare(@"Select distinct hadeeth.subject from hadeeth"))
                    {
                        while (statement.Step() == SQLiteResult.ROW)
                        {
                            list.Add(new Hadith()
                            {
                                Hadeeth = (string)statement[0]
                            });

                            var unBold = new unBold();
                            listSubject.Items.Add(unBold.unBoldHadith(statement[0].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var cd = new ContentDialog();
                cd.Title = "خطأ";
                cd.Content = ex.ToString();
                cd.PrimaryButtonText = "تم";
                await cd.ShowAsync();
            }

            return list;
        }

        private void barbtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void barbtnSearchByAuthor_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchByAuthor));
        }

        private void barbtnSearchByBook_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SearchByBook));
        }

        private async void listSubject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listSubject.SelectedItem == null) return;

            var selectedHadithSubject = listSubject.SelectedItem.ToString();

            txtHeader.Text = "الأحاديث الخاصة بموضوع " + selectedHadithSubject;
            listSubject.Visibility = Visibility.Collapsed;
            listHadith.Visibility = Visibility.Visible;

            ObservableCollection<Hadith> listSelected = new ObservableCollection<Hadith>();

            try
            {
                using (var connection = new SQLiteConnection("hadeeth.sqlite"))
                {
                    using (var statement = connection.Prepare(@"Select hadeeth.hadeeth from hadeeth where hadeeth.subject like '%" + selectedHadithSubject + "%'"))
                    {
                        while (statement.Step() == SQLiteResult.ROW)
                        {
                            listSelected.Add(new Hadith()
                            {
                                Hadeeth = (string)statement[0]
                            });

                            var unBold = new unBold();
                            listHadith.Items.Add(unBold.unBoldHadith(statement[0].ToString()));

                        }
                    }
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

        private async void listHadith_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var hadith = listHadith.SelectedItem.ToString();

            var cd = new ContentDialog();
            cd.Title = "الحديث";
            cd.Content = hadith;
            cd.PrimaryButtonText = "تم";
            cd.SecondaryButtonText = "مشاركة";
            await cd.ShowAsync();
        }
    }
}
