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

namespace TicTacToeXamarin.Game
{
    public enum GameButtonStates
    {
        Circle,
        Cross,
        Standard
    }

    public enum GameStatus
    {
        Continue,
        Win,
        Tie,
        End,
    }

    public enum GameMessageType
    {
        UpdateView,
        SetPlayer,
        Unknown,
    }
}