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
    public class StatsActivity : AppCompatActivity
    {
        ListView _statsList;

        public StatsActivity()
        {
            _statsList = null;
        }

        protected override void OnCreate( Bundle savedInstanceState )
        {
            base.OnCreate( savedInstanceState );
            SetContentView( Resource.Layout.stats_activity );
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            _statsList = FindViewById<ListView>( Resource.Id.statsList );
            List<GameInfoDB> gameInfoDBList =  GameTools._sqLiteDbManager.selectTable();

            if( gameInfoDBList == null 
                || ( gameInfoDBList != null && gameInfoDBList.Count == 0 ) )
            {
                gameInfoDBList = new List<GameInfoDB>();

                gameInfoDBList.Add(new GameInfoDB() { OpponentDeviceName = "Brak elementów do wyświetlenia!" } );
            }

            _statsList.Adapter = new GameStatsListAdapter( this, gameInfoDBList );
        }


        [Export("OnClearStatsHistoryClick")]
        public void OnClearStatsHistoryClick( View gameBoardButtonView )
        {
            List<GameInfoDB> gameInfoDBList = null;

            if ( GameTools._sqLiteDbManager.removeTable() )
            {
                gameInfoDBList = new List<GameInfoDB>();
                gameInfoDBList.Add( new GameInfoDB() { OpponentDeviceName = "Brak elementów do wyświetlenia!" } );
                _statsList.Adapter = new GameStatsListAdapter(this, gameInfoDBList);

                Toast.MakeText(ApplicationContext, "Wyczyszczono całą historię!", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText( ApplicationContext, "Próba usunięcia historii nie powiodła się!", ToastLength.Short ).Show();
            }
        }

        [Export("OnReturnToMenuClick")]
        public void OnReturnToMenuClick( View gameBoardButtonView )
        {
            Intent menuIntent = new Intent( this, typeof( MenuActivity ) );
            StartActivity( menuIntent );
        }
    }
}