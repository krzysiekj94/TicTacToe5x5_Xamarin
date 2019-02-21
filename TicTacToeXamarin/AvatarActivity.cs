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
    public class AvatarActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.avatar_activity);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
        }

        [Export("OnSettingsActivityReturn")]
        public void OnChangeAvatarClick(View gameBoardButtonView)
        {
            Intent settingsActivityIntent = new Intent(this, typeof( SettingsActivity ));
            StartActivity(settingsActivityIntent);
        }

        [Export("OnAvatarImageButtonClick")]
        public void OnAvatarImageButtonClick(View gameBoardButtonView)
        {
            //TODO change avatar and set into settings activity

            Intent settingsActivityIntent = new Intent(this, typeof(SettingsActivity));
            StartActivity(settingsActivityIntent);
        }
    }
}