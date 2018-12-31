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
    class BluetoothListAdapter : BaseAdapter<BluetoothDeviceInfo>
    {
        Activity context;
        List<BluetoothDeviceInfo> list;

        public BluetoothListAdapter( Activity _context, List<BluetoothDeviceInfo> _list )
        : base()
        {
            this.context = _context;
            this.list = _list;
        }

        public override int Count
        {
            get { return list.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override BluetoothDeviceInfo this[int index]
        {
            get { return list[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; 

		if (view == null)
			view = context.LayoutInflater.Inflate (Resource.Layout.ListItemRow, parent, false);

            BluetoothDeviceInfo item = this [position];
		    view.FindViewById<TextView> (Resource.Id.NameDevice).Text = item.nameDeviceString;
		    view.FindViewById<TextView>(Resource.Id.MacDevice).Text = item.macDeviceString;
            view.FindViewById<TextView>(Resource.Id.StatsOpponentText).Text = GetStatsTextAboutOpponent( item.macDeviceString );

            return view;
        }

        private string GetStatsTextAboutOpponent( string macDeviceString )
        {
            string statsOpponentString = String.Empty;
            List<GameInfoDB> gameInfoDBList = GameTools._sqLiteDbManager.selectTable();
            GameInfoDB gameInfoDB = gameInfoDBList.FindAll( gameInfoDBTemp => gameInfoDBTemp.OpponentDeviceMac == macDeviceString ).FirstOrDefault();

            if( gameInfoDB != null )
            {
                statsOpponentString += "Ostatnia rozgrywka: " + gameInfoDB.lastDateTimeGame + "\n";
                statsOpponentString += "Wygranych: " + gameInfoDB.amountOfYourWin + "\n";
                statsOpponentString += "Porażek: " + gameInfoDB.amountOfOpponentWin + "\n";
            }
            else
            {
                statsOpponentString += "Ostatnia rozgrywka: <brak>\n";
                statsOpponentString += "Wygranych: <brak>\n";
                statsOpponentString += "Porażek: <brak>\n";
            }

            return statsOpponentString;
        } 
    }
}