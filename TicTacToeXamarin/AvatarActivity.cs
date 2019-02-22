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
using TicTacToeXamarin.Database;
using TicTacToeXamarin.Game;

namespace TicTacToeXamarin
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AvatarActivity : AppCompatActivity
    {
        private SettingsDB _settingsDB;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.avatar_activity);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            GetSettings();
        }

        private void GetSettings()
        {
            List<SettingsDB> settingsDBList = GameTools._sqLiteDbManager.selectSettingsTable();
            _settingsDB = null;

            if (settingsDBList != null
               && settingsDBList.Count == 1)
            {
                _settingsDB = settingsDBList.FirstOrDefault();
            }
        }

        [Export("OnSettingsActivityReturn")]
        public void OnChangeAvatarClick( View gameBoardButtonView )
        {
            Intent settingsActivityIntent = new Intent(this, typeof( SettingsActivity ));
            StartActivity(settingsActivityIntent);
        }

        [Export("OnAvatarImageButtonClick")]
        public void OnAvatarImageButtonClick( View gameBoardButtonView )
        {
            GameAvatar gameAvatarValue = GameAvatar.Blue;

            if( gameBoardButtonView != null
                && (gameBoardButtonView is ImageButton) )
            {
                switch( gameBoardButtonView.Id )
                {
                    case Resource.Id.avatar0:
                        gameAvatarValue = GameAvatar.Blue;
                        break;
                    case Resource.Id.avatar1:
                        gameAvatarValue = GameAvatar.Green;
                        break;
                    case Resource.Id.avatar2:
                        gameAvatarValue = GameAvatar.Milky;
                        break;
                    case Resource.Id.avatar3:
                        gameAvatarValue = GameAvatar.Sand;
                        break;
                    case Resource.Id.avatar4:
                        gameAvatarValue = GameAvatar.Wooden1;
                        break;
                    case Resource.Id.avatar5:
                        gameAvatarValue = GameAvatar.Wooden2;
                        break;
                }

                _settingsDB.DeviceAvatarId = (int)gameAvatarValue;

                if (!GameTools._sqLiteDbManager.UpdateSettings(_settingsDB))
                {
                    Toast.MakeText(ApplicationContext, "Zapis obrazka do bazy się nie powiódł!", ToastLength.Short).Show();
                }

                Intent settingsActivityIntent = new Intent(this, typeof(SettingsActivity));
                StartActivity(settingsActivityIntent);
            }
        }
    }
}