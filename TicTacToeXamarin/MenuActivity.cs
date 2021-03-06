﻿using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Interop;

namespace TicTacToeXamarin
{
    [Activity( Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait )]
    public class MenuActivity : AppCompatActivity
    {
        protected override void OnCreate( Bundle savedInstanceState )
        {
            base.OnCreate( savedInstanceState );
            SetContentView( Resource.Layout.menu_activity );
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>( Resource.Id.toolbar );
            SetSupportActionBar( toolbar );
            InitGameTools();
        }

        private void InitGameTools()
        {
            GameTools.InitGameTools();
        }

        [Export("OnSearchOpponentButtonClick")]
        public void OnSearchOpponentButtonClick( View gameBoardButtonView )
        {
            Intent searchIntent = new Intent( this, typeof( SearchActivity ) );
            StartActivity( searchIntent );
        }

        [Export("OnMyStatsButtonClick")]
        public void OnMyStatsButtonClick( View gameBoardButtonView )
        {
            Intent statsIntent = new Intent( this, typeof( StatsActivity ) );
            StartActivity( statsIntent );
        }

        [Export("OnSettingsButtonClick")]
        public void OnSettingsButtonClick( View gameBoardButtonView )
        {
            Intent statsIntent = new Intent( this, typeof( SettingsActivity ) );
            StartActivity(statsIntent);
        }

        [Export("OnCloseAppButtonClick")]
        public void OnCloseAppButtonClick( View gameBoardButtonView )
        {
            FinishAffinity();
        }
    }
}