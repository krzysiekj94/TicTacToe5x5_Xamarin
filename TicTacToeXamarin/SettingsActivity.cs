using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Interop;

namespace TicTacToeXamarin
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingsActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView( Resource.Layout.settings_activity );
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>( Resource.Id.toolbar );
            SetSupportActionBar( toolbar );
            SetAvatar();
        }

        private void SetAvatar()
        {

        }

        [Export("OnSaveSettingsClick")]
        public void OnReturnToMenuClick( View gameBoardButtonView )
        {
            //TODO save changes settings to DB
            Toast.MakeText(ApplicationContext, "Zapisuję zmiany do bazy!", ToastLength.Short).Show();
        }

        [Export("OnChangeAvatarClick")]
        public void OnChangeAvatarClick( View gameBoardButtonView )
        {
            //TODO Select avatar
            Toast.MakeText(ApplicationContext, "Zmieniam awatar!", ToastLength.Short).Show();
        }
    }
}