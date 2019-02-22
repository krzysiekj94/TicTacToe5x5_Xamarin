using SQLite;
using System;

namespace TicTacToeXamarin.Database
{
    public class SettingsDB
    {
        public string DeviceMac { get; set; }
        public string DeviceName { get; set; }
        public int DeviceAvatarId { get; set; }
    }
}