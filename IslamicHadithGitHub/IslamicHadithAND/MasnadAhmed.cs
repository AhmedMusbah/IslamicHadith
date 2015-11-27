using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using System.IO;
using Mono.Data.Sqlite;
using System.Data;
using System.Text.RegularExpressions;
using FlatUI;

namespace IslamicHadithAND
{
    [Activity(Label = "مسند أحمد", Icon = "@drawable/Mohamed", Theme = "@android:style/Theme.Holo.Light")]
    public class MasnadAhmed : Activity
    {
        private ProgressDialog progress;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.SetContentView(IslamicHadithAND.Resource.Layout.Book);

            //ActionBar
            ActionBar.NavigationMode = ActionBarNavigationMode.Standard;

            progress = ProgressDialog.Show(this, "انتظر من فضلك", "يتم تحميل الأحاديث ...", true);

            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(1);
                this.RunOnUiThread(() =>
                {
                    try
                    {
                        string content;
                        using (StreamReader streamReader = new StreamReader(Assets.Open("hadeeth.sqlite")))
                        {
                            content = streamReader.ReadToEnd();
                        }
                        string dbName = "hadeeth.sqlite";
                        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
                        if (!File.Exists(dbPath))
                        {
                            using (Stream source = new StreamReader(Assets.Open("hadeeth.sqlite")).BaseStream)
                            {
                                using (var destination = System.IO.File.Create(dbPath))
                                {
                                    source.CopyTo(destination);
                                }
                            }
                        }
                        DataTable dataTableBook = new DataTable();

                        var connectionString = string.Format("Data Source={0};Version=3;", dbPath);
                        using (var conn = new SqliteConnection((connectionString)))
                        {
                            using (var command = conn.CreateCommand())
                            {
                                conn.Open();
                                command.CommandText = @"SELECT hadeeth.*" +
                              "FROM Books INNER JOIN hadeeth ON Books.ID = hadeeth.BID" +
                              " where hadeeth.hadeeth like '%<%' and " +
                              "books.title like '%مسند أحمد‏%'";

                                command.CommandType = CommandType.Text;
                                SqliteDataAdapter dataAdapter = new SqliteDataAdapter();
                                dataAdapter.SelectCommand = command;
                                dataAdapter.Fill(dataTableBook);
                            }
                        }

                        var data = new List<string>();
                        for (int i = 0; i < dataTableBook.Rows.Count; i++)
                        {
                            data.Add(unBold((dataTableBook.Rows[i]["hadeeth"].ToString())));
                            if (dataTableBook.Rows.Count == 0)
                            {
                                new AlertDialog.Builder(this)
                                  .SetTitle("خطأ")
                                  .SetMessage("لا يوجد نتائج")
                                  .SetPositiveButton("عودة", (senderaa, args) =>
                                  {
                                      // Back
                                  })
                                  .Show();
                            }
                        }

                        var listView = FindViewById<ListView>(IslamicHadithAND.Resource.Id.listBook);
                        listView.Adapter = new ArrayAdapter(this, Resource.Layout.ListViewContents, data);

                        listView.ItemClick += (sender, e) =>
                        {
                            var position = e.Position;
                            var HadithBrowser = new Intent(this, typeof(HadithBrowser));
                            HadithBrowser.PutExtra("Hadith", listView.GetItemAtPosition(position).ToString());
                            if (!listView.GetItemAtPosition(position + 1).Equals(null))
                            {
                                position++;
                                HadithBrowser.PutExtra("HadithNext1", listView.GetItemAtPosition(position).ToString());
                            }
                            if (!listView.GetItemAtPosition(position - 1).Equals(null))
                            {
                                position--;
                                HadithBrowser.PutExtra("HadithPrevious1", listView.GetItemAtPosition(position).ToString());
                            }
                            StartActivity(HadithBrowser);
                        };
                    }
                    catch (Exception ex)
                    {
                        new AlertDialog.Builder(this)
                                  .SetPositiveButton("عودة", (sendera, args) =>
                                  {
                                      // Back
                                  })
                                  .SetTitle("خطأ")
                                  .SetMessage("خطأ في قاعدة البيانات ( " + ex.ToString() + " )")
                                  .Show();
                    }

                    progress.Dismiss();
                });
            })).Start();
        }

        public string unBold(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        /*
		 * attach the menu to the menu button of the device
		 * for this activity
		 */
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);

            MenuInflater inflater = this.MenuInflater;

            inflater.Inflate(Resource.Menu.Settings, menu);

            return true;
        }

        /// <param name="item">The menu item that was selected.</param>
		/// <summary>
		/// This hook is called whenever an item in your options menu is selected.
		/// </summary>
		/// <returns>To be added.</returns>
		public override bool OnOptionsItemSelected(IMenuItem item)
        {
            base.OnOptionsItemSelected(item);

            switch (item.ItemId)
            {
                case Resource.Id.ThemesBlue:
                    {
                        var newTheme = FlatUI.FlatUI.GetTheme("أزرق");
                        ChangeTheme(newTheme);
                        break;
                    }
                case Resource.Id.ThemesDark:
                    {
                        var newTheme = FlatUI.FlatUI.GetTheme("داكن");
                        ChangeTheme(newTheme);
                        break;
                    }
                case Resource.Id.ThemesGreen:
                    {
                        var newTheme = FlatUI.FlatUI.GetTheme("أخضر");
                        ChangeTheme(newTheme);
                        break;
                    }
                case Resource.Id.ThemesOrange:
                    {
                        var newTheme = FlatUI.FlatUI.GetTheme("برتقالي");
                        ChangeTheme(newTheme);
                        break;
                    }
                case Resource.Id.ThemesSnow:
                    {
                        var newTheme = FlatUI.FlatUI.GetTheme("فاتح");
                        ChangeTheme(newTheme);
                        break;
                    }
                case Resource.Id.BackgroundNormal:
                    {
                        var layout = FindViewById(Resource.Id.listBook);
                        layout.SetBackgroundResource(IslamicHadithAND.Resource.Drawable.Background);
                        break;
                    }
                case Resource.Id.BackgroundGray:
                    {
                        var layout = FindViewById(Resource.Id.listBook);
                        layout.SetBackgroundColor(Android.Graphics.Color.DarkGray);
                        break;
                    }
                case Resource.Id.BackgroundBlack:
                    {
                        var layout = FindViewById(Resource.Id.listBook);
                        layout.SetBackgroundColor(Android.Graphics.Color.Black);
                        break;
                    }
                default:
                    break;
            }

            return true;
        }

        void ChangeTheme(FlatTheme theme)
        {
            //Set default them
            FlatUI.FlatUI.DefaultTheme = theme;
            //Change the action bar
            FlatUI.FlatUI.SetActionBarTheme(this, FlatUI.FlatUI.DefaultTheme, false);
            //Change the activity theme
            FlatUI.FlatUI.SetActivityTheme(this, theme);
        }
    }
}