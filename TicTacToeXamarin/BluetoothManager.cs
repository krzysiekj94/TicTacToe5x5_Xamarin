using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TicTacToeXamarin
{
    class BluetoothManager
    {
        BluetoothAdapter _bluetoothAdapter;
        BluetoothDeviceInfo _blueToothOpponentDevice;
        Dictionary<string, string> _bluetoothDevicesDictionary;

        public BluetoothManager()
        {
            _blueToothOpponentDevice = null;
            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if( IsEnableBluetoothAdapter() )
            {
                _bluetoothDevicesDictionary = new Dictionary<string, string>();

                foreach (var device in _bluetoothAdapter.BondedDevices)
                {
                    _bluetoothDevicesDictionary.Add( device.Name, device.Address );
                }
            }
        }

        public bool IsEnableBluetoothAdapter()
        {
            return ( _bluetoothAdapter != null
                && _bluetoothAdapter.IsEnabled );
        }

        public void SetBluetoothDeviceOpponent( BluetoothDeviceInfo bluetoothDeviceOpponentInfo )
        {
            _blueToothOpponentDevice = bluetoothDeviceOpponentInfo;
        }

        public BluetoothDeviceInfo GetBluetoothDeviceOpponent()
        {
            return _blueToothOpponentDevice;
        }

        public Dictionary<string, string> GetBluetoothDevicesDictionary()
        {
            return _bluetoothDevicesDictionary;
        }
    }
}