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
using Android.Webkit;
using FlatUI;

namespace IslamicHadithAND
{
    [Activity(Label = "الحديث الشريف", Icon = "@drawable/Mohamed", Theme = "@android:style/Theme.Holo.Light")]
    public class HadithBrowser : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            int TextSize = 20;

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.HadithBrowser);

            //ActionBar
            ActionBar.NavigationMode = ActionBarNavigationMode.Standard;

            var txtHadith = (TextView)FindViewById(Resource.Id.txtHadith);
            var btnZoomIn = (Button)FindViewById(Resource.Id.btnZoomIn);
            var btnZoomOut = (Button)FindViewById(Resource.Id.btnZoomOut);
            var btnNext = (Button)FindViewById(Resource.Id.btnNext);
            var btnPrevious = (Button)FindViewById(Resource.Id.btnPrevious);
            var btnShare = (Button)FindViewById(Resource.Id.btnShare);
            var web = new WebView(this);

            var Hadith = Intent.GetStringExtra("Hadith") ?? "Data not available";
            if (!Intent.GetStringExtra("HadithNext1").ToString().Equals(null))
            {
                var HadithNext1 = Intent.GetStringExtra("HadithNext1") ?? "Data not available";
                btnNext.Click += delegate
                {
                    txtHadith.Text = HadithNext1;
                };
            }
            if (!Intent.GetStringExtra("HadithPrevious1").ToString().Equals(null))
            {
                var HadithPrevious1 = Intent.GetStringExtra("HadithPrevious1") ?? "Data not available";
                btnPrevious.Click += delegate
                {
                    txtHadith.Text = HadithPrevious1;
                };
            }

            txtHadith.Text = Hadith;

            btnZoomIn.Click += delegate
            {
                TextSize++;
                txtHadith.TextSize = TextSize;
            };

            btnZoomOut.Click += delegate
            {
                TextSize--;
                txtHadith.TextSize = TextSize;
            };

            btnShare.Click += (s, arg) =>
            {
                PopupMenu menu = new PopupMenu(this, btnShare);

                // with Android 4 Inflate can be called directly on the menu
                menu.Inflate(IslamicHadithAND.Resource.Menu.menuShare);

                menu.MenuItemClick += (s1, arg1) =>
                {
                    switch (arg1.Item.ItemId)
                    {
                        case Resource.Id.btnShareFacebook:
                            web.LoadUrl("http://www.facebook.com");
                            break;

                        case Resource.Id.btnShareTwitter:
                            web.LoadUrl("http://www.twitter.com");
                            break;

                        case Resource.Id.btnShareWhatsApp:
                            var whatsapp = new Intent(Intent.ActionSend);
                            whatsapp.PutExtra(Intent.DataString, txtHadith.Text);
                            StartActivity(Intent.CreateChooser(whatsapp, "مشاركة الحديث"));
                            break;

                        case Resource.Id.btnCopyToClipboard:
                            var clipboard = (ClipboardManager)GetSystemService(ClipboardService);
                            var clip = ClipData.NewPlainText("Hadith", txtHadith.Text);
                            clipboard.PrimaryClip = clip;
                            Toast.MakeText(this, "تم", ToastLength.Long).Show();
                            break;

                        default:
                            break;
                    }
                };

                menu.Show();
            };
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
                        var txtHadith = FindViewById(Resource.Id.txtHadith);
                        var layoutTable = FindViewById(Resource.Id.layoutMain);
                        layoutTable.SetBackgroundResource(IslamicHadithAND.Resource.Drawable.Background);
                        txtHadith.SetBackgroundResource(IslamicHadithAND.Resource.Drawable.Background);
                        break;
                    }
                case Resource.Id.BackgroundGray:
                    {
                        var txtHadith = FindViewById(Resource.Id.txtHadith);
                        var layoutTable = FindViewById(Resource.Id.layoutMain);
                        layoutTable.SetBackgroundColor(Android.Graphics.Color.DarkGray);
                        txtHadith.SetBackgroundColor(Android.Graphics.Color.DarkGray);
                        break;
                    }
                case Resource.Id.BackgroundBlack:
                    {
                        var txtHadith = FindViewById(Resource.Id.txtHadith);
                        var layoutTable = FindViewById(Resource.Id.layoutMain);
                        layoutTable.SetBackgroundColor(Android.Graphics.Color.Black);
                        txtHadith.SetBackgroundColor(Android.Graphics.Color.Black);
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