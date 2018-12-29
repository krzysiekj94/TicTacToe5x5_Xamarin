using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
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
        }

        [Export("OnSearchOpponentButtonClick")]
        public void OnSearchOpponentButtonClick( View gameBoardButtonView )
        {
            Intent searchIntent = new Intent( this, typeof( SearchActivity ) );
            StartActivity( searchIntent );
        }

        [Export("OnSettingsButtonClick")]
        public void OnSettingsButtonClick( View gameBoardButtonView )
        {
            //TODO 3. Implement new activity with user settings
        }

        [Export("OnCloseAppButtonClick")]
        public void OnCloseAppButtonClick( View gameBoardButtonView )
        {
            FinishAffinity();
        }
    }
}