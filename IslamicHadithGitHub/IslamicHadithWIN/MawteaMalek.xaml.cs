using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IslamicHadithWIN
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MawteaMalek : Page
    {
        public MawteaMalek()
        {
            this.InitializeComponent();

            HadithContents();
        }

        public async System.Threading.Tasks.Task<ObservableCollection<Hadith>> HadithContents()
        {
            ObservableCollection<Hadith> list = new ObservableCollection<Hadith>();

            try
            {
                using (var connection = new SQLiteConnection("hadeeth.sqlite"))
                {
                    using (var statement = connection.Prepare(@"SELECT hadeeth.hadeeth " +
                              "FROM Books INNER JOIN hadeeth ON Books.ID = hadeeth.BID" +
                              " where hadeeth.hadeeth like '%<%' and " +
                              "books.title like '%مسند أحمد‏%'"))
                    {
                        while (statement.Step() == SQLiteResult.ROW)
                        {
                            list.Add(new Hadith()
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
    }
}
