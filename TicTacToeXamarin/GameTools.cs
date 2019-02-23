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
using TicTacToeXamarin.Database;

namespace TicTacToeXamarin
{
    static class GameTools
    {
        public static BluetoothManager _bluetoothManager;
        public static SQLiteDbManager _sqLiteDbManager;
        public static SettingsDB _opponentSettingsDB;

        static public void InitGameTools()
        {
            _sqLiteDbManager = new SQLiteDbManager();
            _bluetoothManager = new BluetoothManager();
            _opponentSettingsDB = new SettingsDB();
        }
    }
}