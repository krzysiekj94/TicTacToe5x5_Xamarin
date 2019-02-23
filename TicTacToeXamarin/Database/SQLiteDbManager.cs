using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;

namespace TicTacToeXamarin.Database
{
    class SQLiteDbManager
    {
        private string _folderPathString;
        private const string NAME_DB_STRING = "GameInfoDB.db";
        private const int I_DEFAULT_AVATAR_ID = 0; 

        public SQLiteDbManager()
        {
            _folderPathString = System.Environment.GetFolderPath( System.Environment.SpecialFolder.Personal );
            CreateDb();
            InitSettings();
        }

        private void InitSettings()
        {
            List<SettingsDB> listSettingsDB = selectSettingsTable();
            SettingsDB settingsDB = new SettingsDB();
            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if ( listSettingsDB == null 
                || ( listSettingsDB != null && listSettingsDB.Count == 0 ))
            {
                settingsDB.DeviceAvatarId = I_DEFAULT_AVATAR_ID;

                if( bluetoothAdapter != null )
                {
                    settingsDB.DeviceName = bluetoothAdapter.Name;
                    settingsDB.DeviceMac = bluetoothAdapter.Address;
                }

                InsertSettingsInfo(settingsDB);
            }
        }

        public bool CreateDb()
        {
            try
            {
                using( var connection = new SQLiteConnection(System.IO.Path.Combine( _folderPathString, NAME_DB_STRING ) ) )
                {
                    connection.CreateTable<GameInfoDB>();
                    connection.CreateTable<SettingsDB>();
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<SettingsDB> selectSettingsTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folderPathString, NAME_DB_STRING)))
                {
                    return connection.Table<SettingsDB>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }

        public SettingsDB selectSettings()
        {
            List<SettingsDB> settingsDBList = GameTools._sqLiteDbManager.selectSettingsTable();
            SettingsDB settingsDB = null;

            if (settingsDBList != null
               && settingsDBList.Count == 1)
            {
                settingsDB = settingsDBList.FirstOrDefault();
            }

            return settingsDB;
        }

        public bool InsertSettingsInfo(SettingsDB settingsInfoDB)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folderPathString, NAME_DB_STRING)))
                {
                    connection.Insert(settingsInfoDB);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool UpdateSettings( SettingsDB settingsDB )
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folderPathString, NAME_DB_STRING)))
                {
                    connection.Query<SettingsDB>("UPDATE SettingsDB set DeviceMac=?, DeviceName=?, DeviceAvatarId=?",
                        settingsDB.DeviceMac, settingsDB.DeviceName, settingsDB.DeviceAvatarId);

                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool InsertGameInfo( GameInfoDB gameInfoDB )
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine( _folderPathString, NAME_DB_STRING)))
                {
                    connection.Insert( gameInfoDB );
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<GameInfoDB> selectTable()
        {
            try
            {
                using( var connection = new SQLiteConnection( System.IO.Path.Combine( _folderPathString, NAME_DB_STRING ) ) )
                {
                    return connection.Table<GameInfoDB>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }


        public bool updateTable( GameInfoDB gameInfoDB )
        {
            try
            {
                using( var connection = new SQLiteConnection( System.IO.Path.Combine( _folderPathString, NAME_DB_STRING ) ) )
                {
                    connection.Query<GameInfoDB>("UPDATE GameInfoDB set OpponentDeviceMac=?, OpponentDeviceName=?, amountOfOpponentWin=?, amountOfYourWin=?, lastDateTimeGame=? Where Id=?", 
                        gameInfoDB.OpponentDeviceMac, gameInfoDB.OpponentDeviceName, gameInfoDB.amountOfOpponentWin, gameInfoDB.amountOfYourWin, gameInfoDB.lastDateTimeGame, gameInfoDB.Id );

                    return true;
                }
            }
            catch( SQLiteException ex )
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool removeTable()
        {
            try
            {
                using (var connection = new SQLiteConnection( System.IO.Path.Combine( _folderPathString, NAME_DB_STRING ) ) )
                {
                    connection.DeleteAll<GameInfoDB>();
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool selectTable( int Id )
        {
            try
            {
                using( var connection = new SQLiteConnection( System.IO.Path.Combine( _folderPathString, NAME_DB_STRING ) ) )
                {
                    connection.Query<GameInfoDB>("SELECT * FROM GameInfoDB Where Id=?", Id);
                    return true;
                }
            }
            catch ( SQLiteException ex )
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }
    }
}