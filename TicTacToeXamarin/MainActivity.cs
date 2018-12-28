using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Interop;
using TicTacToeXamarin.Game;

namespace TicTacToeXamarin
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int SIZE_OF_BOARD_VALUE = 5;
        private const int COMPLETE_TILE_TO_WIN_VALUE = 3;
        private TableLayout _gameBoardTableLayout;
        private Dictionary<int, GameButtonStates> _gameBoardDictionary;
        private GameButtonStates[,] _gameBoardArray;
        private GameButtonStates _symbolGamer;
        int iScoreOfCirclePlayer;
        int iScoreOfCrossPlayer;
        int iAmountOfMoves;

        public MainActivity()
        {
            _symbolGamer = GameButtonStates.Circle;
            iScoreOfCirclePlayer = 0;
            iScoreOfCrossPlayer = 0;
            iAmountOfMoves = 0;
        }

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
            _gameBoardArray = new GameButtonStates[SIZE_OF_BOARD_VALUE, SIZE_OF_BOARD_VALUE];
            _gameBoardDictionary = new Dictionary<int, GameButtonStates>();
            _gameBoardTableLayout = FindViewById<TableLayout>(Resource.Id.boardTableLayout);
            ClearGameBoardArray();
            GenerateBoardButtonsID();
            SetStandardImageForBoardButtons();
        }

        private void ClearGameBoard()
        {
            ClearGameBoardArray();
            ResetGameBoardDictionary();
            SetStandardImageForBoardButtons();
            iAmountOfMoves = 0;
        }

        private void ResetGameBoardDictionary()
        {
            List<int> gameBoardDictionaryKeysList = _gameBoardDictionary.Keys.ToList();

            foreach (int iGameBoardElementId in gameBoardDictionaryKeysList)
            {
                _gameBoardDictionary[iGameBoardElementId] = GameButtonStates.Standard;
            }
        }

        private void ClearGameBoardArray()
        {
            for (int iBoardIterator = 0; iBoardIterator < SIZE_OF_BOARD_VALUE; iBoardIterator++)
            {
                for (int jBoardIterator = 0; jBoardIterator < SIZE_OF_BOARD_VALUE; jBoardIterator++)
                {
                    _gameBoardArray[iBoardIterator, jBoardIterator] = GameButtonStates.Standard;
                }
            }
        }

        private void Win()
        {
            if( _symbolGamer == GameButtonStates.Circle )
            {
                iScoreOfCirclePlayer++;
                //label1.Text = Convert.ToString(score1);
            }
            else
            {
                iScoreOfCrossPlayer++;
                //label3.Text = Convert.ToString(score2);
            }

            using( var alertDialogBuilder = new Android.Support.V7.App.AlertDialog.Builder( this ) )
            {
                string tieTitleString = "Czy chcesz zagrać ponownie:";
                Android.Support.V7.App.AlertDialog myCustomDialog = null;

                alertDialogBuilder.SetTitle(tieTitleString);
                alertDialogBuilder.SetPositiveButton( "Tak", OkAction );
                alertDialogBuilder.SetNegativeButton( "Nie", CancelAction );
                myCustomDialog = alertDialogBuilder.Create();
                myCustomDialog.Show();
            }
        }

        private void OkAction( object sender, DialogClickEventArgs e )
        {
            ClearGameBoard();

            if ( _symbolGamer == GameButtonStates.Cross )
            {
                _symbolGamer = GameButtonStates.Circle;
            }
            else
            {
                _symbolGamer = GameButtonStates.Cross;
            }

            Toast.MakeText( ApplicationContext, "Gracz 1: " + iScoreOfCirclePlayer + "\nGracz 2: " + iScoreOfCrossPlayer, ToastLength.Short ).Show();
        }

      private void CancelAction( object sender, DialogClickEventArgs e )
      {
            ClearGameBoard();
            Toast.MakeText(ApplicationContext, "Koniec gry!", ToastLength.Long).Show();
      }

        private void GenerateBoardButtonsID()
        {
            Context applicationContext = Application.Context;
            TableRow viewTableRow = null;
            View gameRowView = null;
            int iIdTile = 0;
            ImageButton imageButton;

            for( int iRowIterator = 0; iRowIterator < _gameBoardTableLayout.ChildCount; iRowIterator++ )
            {
                gameRowView = _gameBoardTableLayout.GetChildAt(iRowIterator);

                if (gameRowView != null && (gameRowView is TableRow))
                {
                    viewTableRow = (TableRow)gameRowView;
                    for (int iTabRowChildIterator = 0; iTabRowChildIterator < viewTableRow.ChildCount; iTabRowChildIterator++)
                    {
                        imageButton = (ImageButton)viewTableRow.GetChildAt(iTabRowChildIterator);
                        imageButton.Id = iIdTile;
                        _gameBoardDictionary.Add(iIdTile, GameButtonStates.Standard);
                        iIdTile++;
                    }
                }
            }
        }

        private void SetStandardImageForBoardButtons()
        {
            ImageButton boardImageButton = null;

            foreach (var gameDictionaryElement in _gameBoardDictionary)
            {
                boardImageButton = (ImageButton)_gameBoardTableLayout.FindViewById( gameDictionaryElement.Key );

                if( boardImageButton != null )
                {
                    switch( gameDictionaryElement.Value )
                    {
                        case GameButtonStates.Standard:
                            boardImageButton.SetImageResource( Resource.Mipmap.ic_launcher );
                            break;
                        case GameButtonStates.Circle:
                            break;
                        case GameButtonStates.Cross:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public override bool OnCreateOptionsMenu( IMenu menu )
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        [Export("OnButtonClick")]
        public void OnButtonClick( View gameBoardButtonView )
        {
            GameButtonStates gameButtonState = GameButtonStates.Standard;
            ImageButton imageButton = null;
            int[] boardButtonPoint = new int[2];

            if( gameBoardButtonView != null 
                && ( gameBoardButtonView is ImageButton )
                && _gameBoardDictionary.ContainsKey( gameBoardButtonView.Id ) )
            {
                gameButtonState = _gameBoardDictionary[gameBoardButtonView.Id];
                imageButton = FindViewById<ImageButton>( gameBoardButtonView.Id );
                boardButtonPoint = GetButtonCoordinatePointByID( gameBoardButtonView.Id );

                if( imageButton != null 
                    && gameButtonState == GameButtonStates.Standard )
                {
                    iAmountOfMoves++;
                    _gameBoardArray[ boardButtonPoint[0], boardButtonPoint[1] ] = _symbolGamer;

                    switch( _symbolGamer )
                    {
                        case GameButtonStates.Circle:
                            imageButton.SetImageResource( Resource.Mipmap.circle );
                            _gameBoardDictionary[ gameBoardButtonView.Id ] = GameButtonStates.Circle;
                            break;
                        case GameButtonStates.Cross:
                            imageButton.SetImageResource( Resource.Mipmap.cross );
                            _gameBoardDictionary[gameBoardButtonView.Id] = GameButtonStates.Cross;
                            break;
                        default:
                            break;
                    }

                    switch( GetGameStatus( boardButtonPoint ) )
                    {
                        case GameStatus.Continue:
                            SetSymbolNextGamer();
                            break;
                        case GameStatus.End:
                            EndGame();
                            break;
                        case GameStatus.Tie:
                            Tie();
                            break;
                        case GameStatus.Win:
                            Win();
                            break;

                    }
                }
            }
        }

        private void EndGame()
        {
            throw new NotImplementedException();
        }

        private void Tie()
        {
            using( var builder = new Android.Support.V7.App.AlertDialog.Builder( this ) )
            {
                string tieTitleString = "Remis. Czy chcesz zagrać ponownie?";
                Android.Support.V7.App.AlertDialog myCustomDialog = null;

                builder.SetTitle( tieTitleString );
                builder.SetPositiveButton( "Tak", OkAction );
                builder.SetNegativeButton( "Nie", CancelAction );

                myCustomDialog = builder.Create();
                myCustomDialog.Show();
            }
        }

        private void SetSymbolNextGamer()
        {
            switch(_symbolGamer)
            {
                case GameButtonStates.Circle:
                    _symbolGamer = GameButtonStates.Cross;
                    break;
                case GameButtonStates.Cross:
                    _symbolGamer = GameButtonStates.Circle;
                    break;
                default:
                    break;
            }
        }

        private GameStatus GetGameStatus(int[] boardButtonPoint)
        {
            GameStatus eGameStatus = GameStatus.Continue;

            eGameStatus = CheckTie();

            if( eGameStatus == GameStatus.Continue )
            {
                eGameStatus = CheckWin(boardButtonPoint);
            }

            return eGameStatus;
        }

        private GameStatus CheckTie()
        {
            GameStatus eGameStatus = GameStatus.Continue;

            if( iAmountOfMoves == 25 )
            {
                eGameStatus = GameStatus.Tie;
            }

            return eGameStatus;
        }

        private GameStatus CheckWin( int[] boardButtonPoint )
        {
            GameStatus eWinStatus = CheckHorizontally();

            if( eWinStatus != GameStatus.Win )
            {
                eWinStatus = CheckVertically();

                if( eWinStatus != GameStatus.Win )
                {
                    eWinStatus = CheckCrossing( boardButtonPoint );
                }
            }

            return eWinStatus;
        }

        private GameStatus CheckCrossing( int[] boardButtonPoint )
        {
            GameStatus eWinStatus = GameStatus.Continue;
            Dictionary<int, int> rememberDictionary = new Dictionary<int, int>();

            if( _gameBoardArray[ boardButtonPoint[0], boardButtonPoint[1] ] == _symbolGamer )
            {
                rememberDictionary = GetRememberDictionaryElements(boardButtonPoint);

                if( rememberDictionary.Count >= COMPLETE_TILE_TO_WIN_VALUE )
                {
                    rememberDictionary = SortRememberElementsDictionaryByKey( rememberDictionary );
                    eWinStatus = GetStatusFromCrossing( rememberDictionary );
                }
            }

            return eWinStatus;
        }

        private GameStatus GetStatusFromCrossing( Dictionary<int, int> rememberDictionary )
        {
            List<int> rememberDictionaryList = rememberDictionary.Keys.ToList();
            List<int> rememberDictionaryValueList = rememberDictionary.Values.ToList();
            int iCrossNeighbourCounter = 0;
            GameStatus eWinStatus = GameStatus.Continue;

            for (int iRememberElemDictCounter = 1; iRememberElemDictCounter < rememberDictionaryList.Count; iRememberElemDictCounter++)
            {
                if (IsRightDifferenceBetweenCoordinate(rememberDictionaryList, rememberDictionaryValueList, iRememberElemDictCounter))
                {
                    iCrossNeighbourCounter++;

                    if (iCrossNeighbourCounter == 2
                        && IsRightDifferenceExtremeElements(rememberDictionaryList, rememberDictionaryValueList))
                    {
                        eWinStatus = GameStatus.Win;
                        break;
                    }
                }
                else
                {
                    iCrossNeighbourCounter = 0;
                    continue;
                }
            }

            return eWinStatus;
        }

        private Dictionary<int, int> GetRememberDictionaryElements(int[] boardButtonPoint )
        {
            Dictionary<int, int> rememberDictionary = new Dictionary<int, int>();

            int[] boardCombinationXArray = { boardButtonPoint[0] - 1,
                                             boardButtonPoint[0] - 2,
                                             boardButtonPoint[0] + 1,
                                             boardButtonPoint[0] + 2 };

            int[] boardCombinationYArray = { boardButtonPoint[1] - 1,
                                             boardButtonPoint[1] - 2,
                                             boardButtonPoint[1] + 1,
                                             boardButtonPoint[1] + 2 };

            rememberDictionary.Add(boardButtonPoint[0], boardButtonPoint[1]);

            for (int iCounterCombinationX = 0; iCounterCombinationX < boardCombinationXArray.Length; iCounterCombinationX++)
            {
                if (boardCombinationXArray[iCounterCombinationX] >= 0
                    && boardCombinationXArray[iCounterCombinationX] < SIZE_OF_BOARD_VALUE)
                {
                    for (int iCounterCombinationY = 0; iCounterCombinationY < boardCombinationYArray.Length; iCounterCombinationY++)
                    {
                        if (boardCombinationYArray[iCounterCombinationY] >= 0
                            && boardCombinationYArray[iCounterCombinationY] < SIZE_OF_BOARD_VALUE)
                        {
                            if (_gameBoardArray[boardCombinationXArray[iCounterCombinationX], boardCombinationYArray[iCounterCombinationY]] == _symbolGamer)
                            {
                                if (!rememberDictionary.ContainsKey(boardCombinationXArray[iCounterCombinationX]))
                                {
                                    rememberDictionary.Add(boardCombinationXArray[iCounterCombinationX], boardCombinationYArray[iCounterCombinationY]);
                                }
                            }
                        }
                    }
                }
            }

            return rememberDictionary;
        }

        private bool IsRightDifferenceBetweenCoordinate(List<int> rememberDictionaryList, List<int> rememberDictionaryValueList, int iRememberElemDictCounter)
        {
            return Math.Abs(rememberDictionaryList[iRememberElemDictCounter - 1] - rememberDictionaryList[iRememberElemDictCounter]) == 1
                            && Math.Abs(rememberDictionaryValueList[iRememberElemDictCounter - 1] - rememberDictionaryValueList[iRememberElemDictCounter]) == 1;
        }

        private bool IsRightDifferenceExtremeElements(List<int> rememberDictionaryList, List<int> rememberDictionaryValueList)
        {
           return ( Math.Abs(rememberDictionaryList[0] - rememberDictionaryList[rememberDictionaryList.Count - 1]) == 2 )
            && ( Math.Abs(rememberDictionaryValueList[0] - rememberDictionaryValueList[rememberDictionaryList.Count - 1]) == 2 );
        }

        private Dictionary<int, int> SortRememberElementsDictionaryByKey( Dictionary<int, int> rememberDictionary )
        {
           return rememberDictionary.OrderBy(key => key.Key)
                .ToDictionary(c => c.Key, d => d.Value);
        }

        private bool IsTheSameYCoordination( Dictionary<int, int> rememberCoordinationDictionary )
        {
            var lookupTheSameCoordinationDictionary = rememberCoordinationDictionary
                .ToLookup(x => x.Value, x => x.Key)
                .Where( x => x.Count() > 1 );

            return ( lookupTheSameCoordinationDictionary != null );

        }

        private GameStatus CheckVertically()
        {
            GameStatus eWinStatus = GameStatus.Continue;
            int iCountSameSymbols = 0;

            for( int iIteratorColumn = 0; iIteratorColumn < SIZE_OF_BOARD_VALUE; iIteratorColumn++ )
            {   
                for( int iIteratorRow = 0; iIteratorRow < SIZE_OF_BOARD_VALUE; iIteratorRow++ )
                {
                    if( _gameBoardArray[ iIteratorRow, iIteratorColumn ] == _symbolGamer )
                    {
                        iCountSameSymbols++;

                        if( iCountSameSymbols == 3 )
                        {
                            eWinStatus = GameStatus.Win;
                            break;
                        }
                    }
                    else
                    {
                        iCountSameSymbols = 0;
                    }

                    if (eWinStatus == GameStatus.Win)
                    {
                        break;
                    }
                }
            }

            return eWinStatus;
        }

        private GameStatus CheckHorizontally()
        {
            GameStatus eWinStatus = GameStatus.Continue;
            int iCountSameSymbols = 0;

            for (int iIteratorRow = 0; iIteratorRow < SIZE_OF_BOARD_VALUE; iIteratorRow++)
            {
                for (int iIteratorColumn = 0; iIteratorColumn < SIZE_OF_BOARD_VALUE; iIteratorColumn++)
                {
                    if (_gameBoardArray[iIteratorRow, iIteratorColumn] == _symbolGamer)
                    {
                        iCountSameSymbols++;

                        if (iCountSameSymbols == 3)
                        {
                            eWinStatus = GameStatus.Win;
                            break;
                        }
                    }
                    else
                    {
                        iCountSameSymbols = 0;
                    }
                }

                if (eWinStatus == GameStatus.Win)
                {
                    break;
                }
            }

            return eWinStatus;
        }

        private int[] GetButtonCoordinatePointByID( int iButtonId )
        {
            int iCounterValue = 0;
            bool bIsGetCoordinate = false;
            int[] boardButtonCoordinatePointArray = new int[2];

            for( int iIteratorX = 0; iIteratorX < SIZE_OF_BOARD_VALUE; iIteratorX++ )
            {
                for( int iIteratorY = 0; iIteratorY < SIZE_OF_BOARD_VALUE; iIteratorY++)
                {
                    if( iButtonId == iCounterValue)
                    {
                        boardButtonCoordinatePointArray[ 0 ] = iIteratorX;
                        boardButtonCoordinatePointArray[ 1 ] = iIteratorY;
                        bIsGetCoordinate = true;
                        break;
                    }

                    iCounterValue++;
                }

                if( bIsGetCoordinate )
                {
                    break;
                }
            }

            return boardButtonCoordinatePointArray;
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

