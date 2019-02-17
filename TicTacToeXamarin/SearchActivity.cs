using System.Collections.Generic;
using System.Linq;
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
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SearchActivity : AppCompatActivity
    {
        ListView _bluetoothDevicesList;
        BluetoothManager _bluetoothManager;
        List<BluetoothDeviceInfo> _bluetoothDeviceInfoList;
        BluetoothDeviceInfo _currentSelectedDevice;

        public SearchActivity()
        {
            _bluetoothDevicesList = null;
            _currentSelectedDevice = null;
            _bluetoothManager = new BluetoothManager();
            _bluetoothDeviceInfoList = new List<BluetoothDeviceInfo>();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.search_activity);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar( toolbar );
            _bluetoothDevicesList = FindViewById<ListView>( Resource.Id.bluetoothDevicesList );

            LoadBluetoothDeviceList();

            _bluetoothDevicesList.ItemClick += OnListItemClick;
            _bluetoothDevicesList.Adapter = new BluetoothListAdapter( this, _bluetoothDeviceInfoList );
        }

        private void LoadBluetoothDeviceList()
        {
            if( _bluetoothManager.IsEnableBluetoothAdapter() )
            {
                foreach (var device in _bluetoothManager.GetBluetoothDevicesDictionary())
                {
                    _bluetoothDeviceInfoList.Add(new BluetoothDeviceInfo()
                    {
                        nameDeviceString = device.Key,
                        macDeviceString = device.Value
                    });
                }
            }
            else
            {
                Toast.MakeText(Application.Context, "Please enable bluetooth adapter on your device!", ToastLength.Long).Show();
            }
        }

        private void OnListItemClick( object sender, AdapterView.ItemClickEventArgs eventClickItem )
        {
            if( eventClickItem != null )
            {
                _currentSelectedDevice = _bluetoothDeviceInfoList.ElementAt( eventClickItem.Position );
            }
        }

        [Export("OnBluetoothDeviceClick")]
        public void OnBluetoothDeviceClick( View gameBoardButtonView )
        {
            GameTools._bluetoothManager.SetBluetoothDeviceOpponent(_currentSelectedDevice);
            Intent gameIntent = new Intent( this, typeof( GameActivity ) );
            StartActivity( gameIntent );
        }
    }
}