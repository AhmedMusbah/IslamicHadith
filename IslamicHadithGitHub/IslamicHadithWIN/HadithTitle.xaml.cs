using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IslamicHadithWIN
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HadithTitle : Page
    {
        public HadithTitle()
        {
            this.InitializeComponent();

            HadithTitleContents();
        }

        public async System.Threading.Tasks.Task<ObservableCollection<Hadith>> HadithTitleContents()
        {
            listTitle.Visibility = Visibility.Visible;
            listHadith.Visibility = Visibility.Collapsed;

            ObservableCollection<Hadith> list = new ObservableCollection<Hadith>();

            try
            {
                using (var connection = new SQLiteConnection("hadeeth.sqlite"))
                {
                    using (var statement = connection.Prepare(@"Select distinct hadeeth.Title from hadeeth"))
                    {
                        while (statement.Step() == SQLiteResult.ROW)
                        {
                            list.Add(new Hadith()
                            {
                                Hadeeth = (string)statement[0]
                            });

                            var unBold = new unBold();
                            listTitle.Items.Add(unBold.unBoldHadith(statement[0].ToString()));

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

        private async void listTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listTitle.SelectedItem == null) return;

            var selectedHadithTitle = listTitle.SelectedItem.ToString();

            txtHeader.Text = "الحديث الخاص بالعنوان  " + selectedHadithTitle;
            listTitle.Visibility = Visibility.Collapsed;
            listHadith.Visibility = Visibility.Visible;

            ObservableCollection<Hadith> listSelected = new ObservableCollection<Hadith>();

            try
            {
                using (var connection = new SQLiteConnection("hadeeth.sqlite"))
                {
                    using (var statement = connection.Prepare(@"Select hadeeth.hadeeth from hadeeth where hadeeth.Title like '%" + selectedHadithTitle + "%'"))
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
