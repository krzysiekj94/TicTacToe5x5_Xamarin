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
    public class SettingsActivity : AppCompatActivity
    {
        private ImageView _avatarImageView;
        private EditText _nicknameEditText;
        private SettingsDB _settingsDB;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView( Resource.Layout.settings_activity );
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>( Resource.Id.toolbar );
            SetSupportActionBar( toolbar );
            _avatarImageView = FindViewById<ImageView>(Resource.Id.avatarImageView);
            _nicknameEditText = FindViewById<EditText>(Resource.Id.nicknameTextEdit);
            GetSettings();
            SetAvatar();
            SetNickName();
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

        private void SetNickName()
        {
            if( _settingsDB != null )
            {
                _nicknameEditText.Text = _settingsDB.DeviceName;
            }
            else
            {
                _nicknameEditText.Text = "player1";
            }
        }

        private void SetAvatar()
        {
            int avatarID = 0;

            if(_settingsDB != null )
            {
                avatarID = _settingsDB.DeviceAvatarId;
            }

            SetAvatarImage((GameAvatar)avatarID);
        }

        private void SetAvatarImage( GameAvatar avatarID )
        {
            switch( avatarID )
            {
                case GameAvatar.Blue:
                    _avatarImageView.SetImageResource(Resource.Mipmap.blue);
                    break;
                case GameAvatar.Green:
                    _avatarImageView.SetImageResource(Resource.Mipmap.green);
                    break;
                case GameAvatar.Milky:
                    _avatarImageView.SetImageResource(Resource.Mipmap.milky);
                    break;
                case GameAvatar.Sand:
                    _avatarImageView.SetImageResource(Resource.Mipmap.sand);
                    break;
                case GameAvatar.Wooden1:
                    _avatarImageView.SetImageResource(Resource.Mipmap.wooden1);
                    break;
                case GameAvatar.Wooden2:
                    _avatarImageView.SetImageResource(Resource.Mipmap.wooden2);
                    break;
            }
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
            Intent avatarActivityIntent = new Intent( this, typeof( AvatarActivity ) );
            StartActivity( avatarActivityIntent );
        }
    }
}