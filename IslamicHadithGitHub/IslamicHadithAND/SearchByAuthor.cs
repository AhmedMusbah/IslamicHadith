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
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using System.Threading;
using FlatUI;

namespace IslamicHadithAND
{
    [Activity(Label = "بحث باسم الراوي", Icon = "@android:drawable/ic_search_category_default", Theme = "@android:style/Theme.Holo.Light")]
    public class SearchByAuthor : Activity
    {
        ProgressDialog progress;
        string txtAuthorNameData = "";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.SearchByAuthor);

            //ActionBar
            ActionBar.NavigationMode = ActionBarNavigationMode.Standard;

            var btnSearchByAuthorData = FindViewById<Button>(Resource.Id.btnSearchByAuthorData);
            var txtKeywordAuthor = FindViewById<EditText>(Resource.Id.txtKeywordAuthor);
            var spinnerAuthorName = FindViewById<Spinner>(Resource.Id.txtAuthorNameData);

            spinnerAuthorName.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerAuthorName_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.AuthorsNames, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerAuthorName.Adapter = adapter;

            btnSearchByAuthorData.Click += delegate 
            {
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
                            DataTable dataTable = new DataTable();

                            var connectionString = string.Format("Data Source={0};Version=3;", dbPath);
                            using (var sqliteConnection = new SqliteConnection((connectionString)))
                            {
                                using (var command = sqliteConnection.CreateCommand())
                                {
                                    sqliteConnection.Open();
                                    command.CommandText = @"SELECT hadeeth.* " +
                                    "FROM (authors INNER JOIN books ON authors.ID = Books.Author) " +
                                    "INNER JOIN hadeeth ON Books.ID = hadeeth.BID " +
                                    "WHERE (((authors.name) Like '%" + txtAuthorNameData.ToString() + "%') " +
                                    "and ((hadeeth.hadeeth_norm) Like '%" + txtKeywordAuthor.Text + "%'))";

                                    command.CommandType = CommandType.Text;
                                    SqliteDataAdapter dataAdapter = new SqliteDataAdapter();
                                    dataAdapter.SelectCommand = command;
                                    dataAdapter.Fill(dataTable);
                                }
                            }

                            var data = new List<string>();
                            for (int i = 0; i < dataTable.Rows.Count; i++)
                            {
                                data.Add(unBold(dataTable.Rows[i]["hadeeth"].ToString()));
                            }

                            if (dataTable.Rows.Count == 0)
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

                            var listView = FindViewById<ListView>(Resource.Id.listHadithByAuthor);
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
            };
        }

        private void spinnerAuthorName_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            txtAuthorNameData = spinner.GetItemAtPosition(e.Position).ToString();
            if (txtAuthorNameData=="بحث لكل الرواه")
            {
                txtAuthorNameData = "<";
            }
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
                        var layout = FindViewById(Resource.Id.listHadithByAuthor);
                        layout.SetBackgroundResource(IslamicHadithAND.Resource.Drawable.Background);
                        TextView b1 = (TextView)FindViewById(Resource.Id.txtKeywordAuthor);
                        Spinner b2 = (Spinner)FindViewById(Resource.Id.txtAuthorNameData);
                        Button b3 = (Button)FindViewById(Resource.Id.btnSearchByAuthorData);
                        b1.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        b1.SetTextColor(Android.Graphics.Color.White);
                        b2.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        b3.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        b3.SetTextColor(Android.Graphics.Color.White);
                        break;
                    }
                case Resource.Id.BackgroundGray:
                    {
                        var layout = FindViewById(Resource.Id.listHadithByAuthor);
                        layout.SetBackgroundColor(Android.Graphics.Color.DarkGray);
                        TextView b1 = (TextView)FindViewById(Resource.Id.txtKeywordAuthor);
                        Spinner b2 = (Spinner)FindViewById(Resource.Id.txtAuthorNameData);
                        Button b3 = (Button)FindViewById(Resource.Id.btnSearchByAuthorData);
                        b1.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        b1.SetTextColor(Android.Graphics.Color.White);
                        b2.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        b3.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        b3.SetTextColor(Android.Graphics.Color.White);
                        break;
                    }
                case Resource.Id.BackgroundBlack:
                    {
                        var layout = FindViewById(Resource.Id.listHadithByAuthor);
                        layout.SetBackgroundColor(Android.Graphics.Color.Black);
                        TextView b1 = (TextView)FindViewById(Resource.Id.txtKeywordAuthor);
                        Spinner b2 = (Spinner)FindViewById(Resource.Id.txtAuthorNameData);
                        Button b3 = (Button)FindViewById(Resource.Id.btnSearchByAuthorData);
                        b1.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        b1.SetTextColor(Android.Graphics.Color.White);
                        b2.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        b3.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        b3.SetTextColor(Android.Graphics.Color.White);
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