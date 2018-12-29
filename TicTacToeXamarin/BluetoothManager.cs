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

            if ( _bluetoothAdapter == null )
            {
                throw new Exception("No Bluetooth adapter found.");
            }

            if( !_bluetoothAdapter.IsEnabled )
            {
                throw new Exception("Bluetooth adapter is not enabled.");
            }

            _bluetoothDevicesDictionary = new Dictionary<string, string>();

            foreach ( var device in _bluetoothAdapter.BondedDevices )
            {
                _bluetoothDevicesDictionary.Add( device.Name, device.Address );
            }
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