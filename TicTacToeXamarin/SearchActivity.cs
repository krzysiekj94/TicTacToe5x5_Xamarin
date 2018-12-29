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
using static Android.Graphics.Interpolator;

namespace TicTacToeXamarin
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SearchActivity : AppCompatActivity
    {
        ListView _bluetoothDevicesList;
        BluetoothManager _bluetoothManager;
        List<BluetoothDeviceInfo> _bluetoothDeviceInfoList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.search_activity);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar( toolbar );
            _bluetoothDevicesList = FindViewById<ListView>( Resource.Id.bluetoothDevicesList );
            _bluetoothDeviceInfoList = new List<BluetoothDeviceInfo>();

            _bluetoothManager = new BluetoothManager();

            foreach (var device in _bluetoothManager.GetBluetoothDevicesDictionary() )
            {
                _bluetoothDeviceInfoList.Add( new BluetoothDeviceInfo()
                { name = device.Key,
                    mac = device.Value
                });
            }

            _bluetoothDevicesList.ItemClick += OnListItemClick;
            _bluetoothDevicesList.Adapter = new BluetoothListAdapter(this, _bluetoothDeviceInfoList);
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            BluetoothDeviceInfo item = _bluetoothDeviceInfoList.ElementAt( e.Position );
            // Do whatever you like here
        }
    }
}