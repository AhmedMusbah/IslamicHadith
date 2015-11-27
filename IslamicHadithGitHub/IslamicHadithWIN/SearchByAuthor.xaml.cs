using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IslamicHadithWIN
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchByAuthor : Page
    {
        public SearchByAuthor()
        {
            this.InitializeComponent();
        }

        public async System.Threading.Tasks.Task<ObservableCollection<Hadith>> SearchByAuthorContents()
        {
            var Keyword = txtAuthorKeyword.Text;
            var Author = txtAuthorName.Text;

            ObservableCollection<Hadith> list = new ObservableCollection<Hadith>();

            try
            {
                using (var connection = new SQLiteConnection("hadeeth.sqlite"))
                {
                    using (var statement = connection.Prepare(@"SELECT hadeeth.hadeeth " +
                                    "FROM (authors INNER JOIN books ON authors.ID = Books.Author) " +
                                    "INNER JOIN hadeeth ON Books.ID = hadeeth.BID " +
                                    "WHERE (((authors.name) Like '%" + Author + "%') " +
                                    "and ((hadeeth.hadeeth_norm) Like '%" + Keyword + "%'))"))
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
                cd.Title = ex.ToString();
                cd.PrimaryButtonText = "تم";
                await cd.ShowAsync();
            }

            return list;

        }

        private async void btnSearchByAuthorData_Click(object sender, RoutedEventArgs e)
        {
            await SearchByAuthorContents();
        }

        private void barbtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
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
