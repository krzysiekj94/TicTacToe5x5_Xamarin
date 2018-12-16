using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using TicTacToeXamarin.Game;

namespace TicTacToeXamarin
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TableLayout _gameBoardtableLayout;
        private Dictionary<int, GameButtonStates> _gameBoard;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            InitBoard();
        }

        private void InitBoard()
        {
            TableRow gameTableRow = null;
            Context context = Android.App.Application.Context;
            _gameBoardtableLayout = FindViewById<TableLayout>( Resource.Id.boardTableLayout );
            _gameBoard = new Dictionary<int, GameButtonStates>();

            int id = 0;

            for (int i = 0; i < _gameBoardtableLayout.ChildCount; i++)
            {
                View view = _gameBoardtableLayout.GetChildAt(i);
                TableRow viewRow = null;

                if(view != null && ( view is TableRow ) )
                {
                    viewRow = (TableRow)view;
                    for( int j=0; j < viewRow.ChildCount; j++)
                    {
                        ImageButton imageButton = (ImageButton)viewRow.GetChildAt(j);
                        imageButton.Id = id;
                        _gameBoard.Add(id, GameButtonStates.Standard);
                        id++;
                    }
                }
            }

            foreach( var gameElement in _gameBoard )
            {
                ImageButton imageButton = (ImageButton)_gameBoardtableLayout.FindViewById(gameElement.Key);

                if(imageButton != null)
                {
                    switch (gameElement.Value)
                    {
                        case GameButtonStates.Circle:
                            imageButton.SetImageResource(Resource.Mipmap.circle);
                        break;
                        case GameButtonStates.Cross:
                            imageButton.SetImageResource(Resource.Mipmap.cross);
                            break;
                        case GameButtonStates.Standard:
                            imageButton.SetImageResource(Resource.Mipmap.ic_launcher);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
	}
}

