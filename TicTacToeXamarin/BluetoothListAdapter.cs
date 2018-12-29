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

        public BluetoothListAdapter(Activity _context, List<BluetoothDeviceInfo> _list)
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
		    view.FindViewById<TextView> (Resource.Id.NameDevice).Text = item.name;
		    view.FindViewById<TextView>(Resource.Id.MacDevice).Text = item.mac;

		    return view;
        }
    }
}