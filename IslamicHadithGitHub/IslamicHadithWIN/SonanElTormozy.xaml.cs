using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class SonanElTormozy : Page
    {
        public class ItemsToShow : ObservableCollection<string>, ISupportIncrementalLoading
        {
            public int lastItem = 1;

            public bool HasMoreItems
            {
                get
                {
                    if (lastItem == 100)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }        

            public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
            {
                var coreDispatcher = Window.Current.Dispatcher;

                return System.Threading.Tasks.Task.Run<LoadMoreItemsResult>(async () =>
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {

                        });

                    ObservableCollection<Hadith> list = new ObservableCollection<Hadith>();
                    using (var connection = new SQLiteConnection("hadeeth.sqlite"))
                    {
                        using (var statement = connection.Prepare(@"SELECT hadeeth.hadeeth " +
                              "FROM Books INNER JOIN hadeeth ON Books.ID = hadeeth.BID" +
                              " where hadeeth.hadeeth like '%<%' and " +
                              "books.title like '%سنن الترمذي‏%'"))
                        {
                            while (statement.Step() == SQLiteResult.ROW)
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    list.Add(new Hadith()
                                    {
                                        Hadeeth = (string)statement[0]
                                    });
                                    lastItem++;
                                    if (lastItem == 100)
                                    {
                                        break;
                                    }
                                }

                                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                    () =>
                                    {
                                        foreach (var item in list)
                                        {
                                            var unBold = new unBold();
                                            var x = new SonanElTormozy();
                                            x.listHadith.Items.Add(unBold.unBoldHadith(statement[0].ToString()));
                                        }
                                    });
                            }
                        }
                    }

                    return new LoadMoreItemsResult() { Count = count };
                }).AsAsyncOperation<LoadMoreItemsResult>();
            }
        }

        public SonanElTormozy()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            listHadith.ItemsSource = new ItemsToShow();
        }

        private async System.Threading.Tasks.Task<ObservableCollection<Hadith>> HadithContents()
        {
            ObservableCollection<Hadith> list = new ObservableCollection<Hadith>();

            try
            {
                using (var connection = new SQLiteConnection("hadeeth.sqlite"))
                {
                    using (var statement = connection.Prepare(@"SELECT hadeeth.hadeeth " +
                              "FROM Books INNER JOIN hadeeth ON Books.ID = hadeeth.BID" +
                              " where hadeeth.hadeeth like '%<%' and " +
                              "books.title like '%سنن الترمذي‏%'"))
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
