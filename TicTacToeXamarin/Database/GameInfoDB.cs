using SQLite;
using System;

namespace TicTacToeXamarin
{
    public class GameInfoDB
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string OpponentDeviceMac { get; set; }
        public string OpponentDeviceName { get; set; }
        public int amountOfOpponentWin { get; set; }
        public int amountOfYourWin { get; set; }
        public string lastDateTimeGame { get; set; }
    }
}