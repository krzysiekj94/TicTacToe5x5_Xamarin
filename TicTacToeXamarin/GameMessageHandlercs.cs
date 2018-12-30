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
    class GameMessageHandler : Handler
    {
        GameActivity _gameActivity;
        
        public GameMessageHandler( GameActivity gameActivity )
        {
            _gameActivity = gameActivity;
        }

        public override void HandleMessage( Message msg )
        {
            switch (msg.What)
            {
                case Constants.MESSAGE_STATE_CHANGE:
                    switch (msg.What)
                    {
                        case TicTacToeXamarin.BluetoothService.STATE_CONNECTED:
                            //Toast.MakeText(Application.Context, "STATE_CONNECTED", ToastLength.Long).Show();
                            break;
                        case TicTacToeXamarin.BluetoothService.STATE_CONNECTING:
                            //Toast.MakeText(Application.Context, "STATE_CONNECTING", ToastLength.Long).Show();
                            break;
                        case TicTacToeXamarin.BluetoothService.STATE_LISTEN:
                            //Toast.MakeText(Application.Context, "STATE_LISTEN", ToastLength.Long).Show();
                            break;
                        case TicTacToeXamarin.BluetoothService.STATE_NONE:
                            //Toast.MakeText(Application.Context, "STATE_NONE", ToastLength.Long).Show();
                            break;
                    }
                    break;
                case Constants.MESSAGE_WRITE:
                    var writeBuffer = ( byte[] )msg.Obj;
                    var writeMessage = Encoding.ASCII.GetString( writeBuffer );
                    //Toast.MakeText(Application.Context, "Wysłałem: " + writeMessage, ToastLength.Long).Show();
                    break;
                case Constants.MESSAGE_READ:
                    var readBuffer = ( byte[] )msg.Obj;
                    var readMessage = Encoding.ASCII.GetString( readBuffer );
                    _gameActivity.RemoteUpdateGameBoard( readMessage );
                    //Toast.MakeText(Application.Context, "Odebrałem: " + readMessage, ToastLength.Long).Show();
                    break;
                case Constants.MESSAGE_DEVICE_NAME:
                    string messagedevice = msg.Data.GetString( Constants.DEVICE_NAME );
                    Toast.MakeText( Application.Context, "Połączono z: " + messagedevice, ToastLength.Long).Show();
                    break;
                case Constants.MESSAGE_TOAST:
                    break;
            }
        }
    }
}