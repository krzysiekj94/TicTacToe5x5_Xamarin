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

namespace TicTacToeXamarin
{
    class GameStatsListAdapter : BaseAdapter<GameInfoDB>
    {
        Activity _activityContext;
        List<GameInfoDB> _gameInfoDBList;

        public GameStatsListAdapter( Activity _context, List<GameInfoDB> _list )
            : base()
        {
            this._activityContext = _context;
            this._gameInfoDBList = _list;
        }

        public override int Count
        {
            get { return _gameInfoDBList.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override GameInfoDB this[int index]
        {
            get
            {
                return _gameInfoDBList[index];
            }
        }

        public override View GetView( int position, View convertView, ViewGroup parent )
        {
            View view = convertView;
            GameInfoDB item = null;

            if( view == null )
            {
                view = _activityContext.LayoutInflater.Inflate( Resource.Layout.GameStatInfoRow, parent, false );
            }

            item = this [ position ];
		    view.FindViewById<TextView>( Resource.Id.NameDevice ).Text = item.OpponentDeviceName;
		    view.FindViewById<TextView>( Resource.Id.MacDevice ).Text = item.OpponentDeviceMac;
            view.FindViewById<TextView>(Resource.Id.WinAmount).Text = item.amountOfYourWin.ToString();
            view.FindViewById<TextView>(Resource.Id.LoseAmount).Text = item.amountOfOpponentWin.ToString();
            view.FindViewById<TextView>(Resource.Id.dateLastGame).Text = item.lastDateTimeGame.ToString();

            return view;
        }
    }
}