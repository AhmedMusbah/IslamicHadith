using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using System.Threading;
using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace IslamicHadithAND
{
    [Activity(Label = "الحديث الشريف", Icon = "@drawable/Mohamed", Theme = "@android:style/Theme.Holo.Light")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MainActivity);

            // set our buttons
            var btnSahihMoslem = FindViewById<Button>(Resource.Id.btnSahihMoslem);
            var btnSahihElBokhary = FindViewById<Button>(Resource.Id.btnSahihElBokhary);
            var btnSonanElNesaay = FindViewById<Button>(Resource.Id.btnSonanElNesaay);
            var btnSonanAbnMagh = FindViewById<Button>(Resource.Id.btnSonanAbnMagh);
            var btnSonanAbyDawod = FindViewById<Button>(Resource.Id.btnSonanAbyDawod);
            var btnSonanElDarmy = FindViewById<Button>(Resource.Id.btnSonanElDarmy);
            var btnSonanElTormozy = FindViewById<Button>(Resource.Id.btnSonanElTormozy);
            var btnMasnadAhmed = FindViewById<Button>(Resource.Id.btnMasnadAhmed);
            var btnMawteaMalek = FindViewById<Button>(Resource.Id.btnMawteaMalek);

            //ActionBar
            ActionBar.NavigationMode = ActionBarNavigationMode.Standard;

            // button event
            btnSahihMoslem.Click += delegate
            {
                var intent = new Intent(this, typeof(SahihMoslem));
                StartActivity(intent);
            };

            btnSahihElBokhary.Click += delegate
            {
                var intent = new Intent(this, typeof(SahihElBokhary));
                StartActivity(intent);
            };

            btnSonanElNesaay.Click += delegate
            {
                var intent = new Intent(this, typeof(SonanElNesaay));
                StartActivity(intent);
            };

            btnSonanAbnMagh.Click += delegate
            {
                var intent = new Intent(this, typeof(SonanAbnMagh));
                StartActivity(intent);
            };

            btnSonanAbyDawod.Click += delegate
            {
                var intent = new Intent(this, typeof(SonanAbyDawod));
                StartActivity(intent);
            };

            btnSonanElTormozy.Click += delegate
            {
                var intent = new Intent(this, typeof(SonanElTormozy));
                StartActivity(intent);
            };

            btnSonanElDarmy.Click += delegate
            {
                var intent = new Intent(this, typeof(SonanElDarmy));
                StartActivity(intent);
            };

            btnMasnadAhmed.Click += delegate
            {
                var intent = new Intent(this, typeof(MasnadAhmed));
                StartActivity(intent);
            };

            btnMawteaMalek.Click += delegate
            {
                var intent = new Intent(this, typeof(MawteaMalek));
                StartActivity(intent);
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

            inflater.Inflate(Resource.Menu.mainmenu, menu);

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

            var web = new WebView(this);

            switch (item.ItemId)
            {
                case Resource.Id.SearchByBook:
                    {
                        var Bookintent = new Intent(this, typeof(SearchByBook));
                        StartActivity(Bookintent);
                        break;
                    }
                case Resource.Id.SearchByAuthor:
                    {
                        var Authorintent = new Intent(this, typeof(SearchByAuthor));
                        StartActivity(Authorintent);
                        break;
                    }
                case Resource.Id.ShowBySubject:
                    {
                        var intentSubject = new Intent(this, typeof(HadithSubject));
                        StartActivity(intentSubject);
                        break;
                    }
                case Resource.Id.ShowByTitle:
                    {
                        var intentTitle = new Intent(this, typeof(HadithTitle));
                        StartActivity(intentTitle);
                        break;
                    }

                case Resource.Id.help:
                    web.LoadUrl("http://www.softlock.net/");
                    break;

                        case Resource.Id.about:
                    web.LoadUrl("http://www.softlock.net/");
                    break;

                default:
                    break;
            }

            return true;
        }
    }
}