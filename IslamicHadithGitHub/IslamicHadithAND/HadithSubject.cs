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
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using EndlessScrollPort;
using FlatUI;
using Android.Graphics;
using System.Threading.Tasks;

namespace IslamicHadithAND
{
    [Activity(Label = "الموضوعات", Icon = "@drawable/Mohamed", Theme = "@android:style/Theme.Holo.Light")]
    public class HadithSubject : Activity
    {
        private ProgressDialog progress;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //ActionBar
            ActionBar.NavigationMode = ActionBarNavigationMode.Standard;

            SetContentView(Resource.Layout.HadithSubject);

            progress = ProgressDialog.Show(this, "انتظر من فضلك", "يتم تحميل الموضوعات ...", true);

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
                        string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
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

                        DataTable dataTableSubject = new DataTable();
                        DataTable dataTableHadith = new DataTable();

                        var connectionString = string.Format("Data Source={0};Version=3;", dbPath);
                        using (var SqliteConnection = new SqliteConnection((connectionString)))
                        {
                            using (var command = SqliteConnection.CreateCommand())
                            {
                                SqliteConnection.Open();
                                command.CommandText = @"select distinct hadeeth.subject from hadeeth";

                                command.CommandType = CommandType.Text;
                                SqliteDataAdapter dataAdapter = new SqliteDataAdapter();
                                dataAdapter.SelectCommand = command;
                                dataAdapter.Fill(dataTableSubject);
                            }
                        }

                        var data = new List<string>();

                        if (dataTableSubject.Rows.Count == 0)
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

                        for (int i = 0; i < dataTableSubject.Rows.Count; i++)
                        {
                            data.Add(unBold(dataTableSubject.Rows[i]["subject"].ToString()));
                        }

                        var listView = FindViewById<ListView>(Resource.Id.listHadithSubject);
                        listView.Adapter = new ArrayAdapter(this, Resource.Layout.ListViewContents, data);

                        listView.ItemClick += (sender, e) =>
                        {
                            progress = ProgressDialog.Show(this, "انتظر من فضلك", "يتم تحميل الاحاديث ...", true);

                            new Thread(new ThreadStart(() =>
                            {
                                Thread.Sleep(1);
                                this.RunOnUiThread(() =>
                                {
                                    var connectionString2 = string.Format("Data Source={0};Version=3;", dbPath);
                                    using (var SqliteConnection = new SqliteConnection((connectionString2)))
                                    {
                                        using (var command = SqliteConnection.CreateCommand())
                                        {
                                            SqliteConnection.Open();
                                            command.CommandText = @"SELECT hadeeth.* " +
                                            "FROM hadeeth WHERE " +
                                            "(((hadeeth.subject) Like '%" +
                                            listView.GetItemAtPosition(e.Position).ToString() + "%'))";

                                            command.CommandType = CommandType.Text;
                                            SqliteDataAdapter dataAdapter = new SqliteDataAdapter();
                                            dataAdapter.SelectCommand = command;
                                            dataAdapter.Fill(dataTableHadith);
                                        }
                                    }

                                    var data2 = new List<string>();
                                    for (int i = 0; i < dataTableHadith.Rows.Count; i++)
                                    {
                                        data2.Add(unBold(dataTableHadith.Rows[i]["hadeeth"].ToString()));
                                    }

                                    if (dataTableHadith.Rows.Count == 0)
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

                                    var listView2 = FindViewById<ListView>(Resource.Id.listHadithSubject);
                                    listView2.Adapter = new ArrayAdapter(this, Resource.Layout.ListViewContents, data2);

                                    listView2.ItemClick += (senderaaa, ee) =>
                                    {
                                        var position = ee.Position;
                                        var HadithBrowser = new Intent(this, typeof(HadithBrowser));
                                        HadithBrowser.PutExtra("Hadith", listView2.GetItemAtPosition(position).ToString());
                                        if (!listView2.GetItemAtPosition(position + 1).Equals(null))
                                        {
                                            position++;
                                            HadithBrowser.PutExtra("HadithNext1", listView2.GetItemAtPosition(position).ToString());
                                        }
                                        if (!listView2.GetItemAtPosition(position - 1).Equals(null))
                                        {
                                            position--;
                                            HadithBrowser.PutExtra("HadithPrevious1", listView2.GetItemAtPosition(position).ToString());
                                        }
                                        StartActivity(HadithBrowser);
                                    };

                                    progress.Dismiss();
                                });
                            })).Start();
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
                        var layout = FindViewById(Resource.Id.listHadithSubject);
                        layout.SetBackgroundResource(IslamicHadithAND.Resource.Drawable.Background);

                        break;
                    }
                case Resource.Id.BackgroundGray:
                    {
                        var layout = FindViewById(Resource.Id.listHadithSubject);
                        layout.SetBackgroundColor(Android.Graphics.Color.DarkGray);
                        break;
                    }
                case Resource.Id.BackgroundBlack:
                    {
                        var layout = FindViewById(Resource.Id.listHadithSubject);
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

        public string unBold(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}