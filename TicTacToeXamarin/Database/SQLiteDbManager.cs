using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
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

        public SQLiteDbManager()
        {
            _folderPathString = System.Environment.GetFolderPath( System.Environment.SpecialFolder.Personal );
            CreateDb();
        }

        public bool CreateDb()
        {
            try
            {
                using( var connection = new SQLiteConnection(System.IO.Path.Combine( _folderPathString, NAME_DB_STRING ) ) )
                {
                    connection.CreateTable<GameInfoDB>();
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

        public bool removeTable( GameInfoDB gameInfoDB )
        {
            try
            {
                using (var connection = new SQLiteConnection( System.IO.Path.Combine( _folderPathString, NAME_DB_STRING ) ) )
                {
                    connection.Delete( gameInfoDB );
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