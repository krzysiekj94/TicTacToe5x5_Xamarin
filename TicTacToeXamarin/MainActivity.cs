using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
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
        private const int SIZE_OF_BOARD = 5;

        private TableLayout _gameBoardTableLayout;
        private Dictionary<int, GameButtonStates> _gameBoardDictionary;

        private static GameButtonStates _symbolGamer = GameButtonStates.Circle;
        int score1 = 0, score2 = 0, ruchy = 0;
        private static bool isWin = false;
        private static GameButtonStates[,] _gameBoardArray;


        private void Win(GameButtonStates zwyciezca)
        {

            if( zwyciezca == GameButtonStates.Circle )
            {
                score1++;
                //label1.Text = Convert.ToString(score1);
            }
            else
            {
                score2++;
                //label3.Text = Convert.ToString(score2);
            }

            using( var builder = new Android.Support.V7.App.AlertDialog.Builder( this ) )
            {
                var title = "Please edit your details:";
                builder.SetTitle(title);
                builder.SetPositiveButton("OK", OkAction);
                builder.SetNegativeButton("Cancel", CancelAction);
                var myCustomDialog = builder.Create();
                myCustomDialog.Show();
            }
        }

        private void OkAction(object sender, DialogClickEventArgs e)
        {
            ClearGameBoard();
            ruchy = 0;

            if ( _symbolGamer == GameButtonStates.Cross )
            {
                _symbolGamer = GameButtonStates.Circle;
            }
            else
            {
                _symbolGamer = GameButtonStates.Cross;
            }

            Toast.MakeText( ApplicationContext, "Gracz 1: " + score1 + "\nGracz 2: " + score2, ToastLength.Short ).Show();
        }

        private void ClearGameBoard()
        {
            ClearGameBoardArray();
            ResetGameBoardDictionary();
            SetStandardImageForBoardButtons();
        }

        private void ResetGameBoardDictionary()
        {
            List<int> gameBoardDictionaryKeysList = _gameBoardDictionary.Keys.ToList();

            foreach( int iGameBoardElementId in gameBoardDictionaryKeysList )
            {
                _gameBoardDictionary[iGameBoardElementId] = GameButtonStates.Standard;
            }
        }

      private void CancelAction( object sender, DialogClickEventArgs e )
      {
            ClearGameBoard();
            Toast.MakeText(ApplicationContext, "Koniec gry!", ToastLength.Long).Show();
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
            _gameBoardArray = new GameButtonStates[ SIZE_OF_BOARD, SIZE_OF_BOARD ];
            _gameBoardDictionary = new Dictionary<int, GameButtonStates>();
            _gameBoardTableLayout = FindViewById<TableLayout>(Resource.Id.boardTableLayout);
            ClearGameBoardArray();
            GenerateBoardButtonsID();
            SetStandardImageForBoardButtons();
        }

        private void GenerateBoardButtonsID()
        {
            Context applicationContext = Application.Context;
            TableRow viewTableRow = null;
            View rowView = null;
            int idTile = 0;
            ImageButton imageButton;

            for( int iRowIterator = 0; iRowIterator < _gameBoardTableLayout.ChildCount; iRowIterator++ )
            {
                rowView = _gameBoardTableLayout.GetChildAt(iRowIterator);

                if (rowView != null && (rowView is TableRow))
                {
                    viewTableRow = (TableRow)rowView;
                    for (int iTabRowChildIterator = 0; iTabRowChildIterator < viewTableRow.ChildCount; iTabRowChildIterator++)
                    {
                        imageButton = (ImageButton)viewTableRow.GetChildAt(iTabRowChildIterator);
                        imageButton.Id = idTile;
                        _gameBoardDictionary.Add(idTile, GameButtonStates.Standard);
                        idTile++;
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

                if(boardImageButton != null )
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

        private void ClearGameBoardArray()
        {
            for (int iBoardIterator = 0; iBoardIterator < SIZE_OF_BOARD; iBoardIterator++)
            {
                for (int jBoardIterator = 0; jBoardIterator < SIZE_OF_BOARD; jBoardIterator++)
                {
                    _gameBoardArray[iBoardIterator, jBoardIterator] = GameButtonStates.Standard;
                }
            }
        }

        public override bool OnCreateOptionsMenu( IMenu menu )
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        private void Remis()
        {
            if( true /*MessageBox.Show("Remis. Zagrac ponownie?", "KONIEC", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes*/ )
            {
                for (int i = 0; i < SIZE_OF_BOARD; i++)
                    for (int j = 0; j < SIZE_OF_BOARD; j++)
                        _gameBoardArray[ i, j ] = GameButtonStates.Standard;

                for( int i = 0; i < SIZE_OF_BOARD; i++ )
                {
                    for( int j = 0; j < SIZE_OF_BOARD; j++ )
                    {
                        _gameBoardArray[ i, j ] = GameButtonStates.Standard;
                    }  
                }

                foreach (var gameElement in _gameBoardDictionary)
                {
                    ImageButton imageButton = (ImageButton)_gameBoardTableLayout.FindViewById(gameElement.Key);
                    imageButton.SetImageResource(Resource.Mipmap.ic_launcher);
                }

                if (_symbolGamer == GameButtonStates.Cross)
                {
                    _symbolGamer = GameButtonStates.Circle;
                }
                else
                {
                    _symbolGamer = GameButtonStates.Cross;
                }

                ruchy = 0;
            }
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
                    ruchy += 1;
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
                            break;
                        case GameStatus.Tie:
                            break;
                        case GameStatus.Win:
                            Win( _symbolGamer );
                            break;

                    }
                    
                    //UpdateTextbox();
                    //Poziomo1( boardButtonPoint[0], boardButtonPoint[1] );
                    //Pionowo1( boardButtonPoint[0], boardButtonPoint[1] );
                    //UkosDol1( boardButtonPoint[0], boardButtonPoint[1] );

                    /*
                    if( isWin == true )
                    {
                        isWin = false;
                        if (_symbolGamer == GameButtonStates.Circle)
                        {
                            imageButton.SetImageResource( Resource.Mipmap.cross );
                            _gameBoardDictionary[gameBoardButtonView.Id] = GameButtonStates.Cross;
                        }
                        else
                        {
                            imageButton.SetImageResource( Resource.Mipmap.circle );
                            _gameBoardDictionary[gameBoardButtonView.Id] = GameButtonStates.Circle;
                        }
                            
                        Win( _symbolGamer );
                    }
                    else
                    {
                        if( _symbolGamer == GameButtonStates.Circle)
                        {
                            _symbolGamer = GameButtonStates.Cross;
                            imageButton.SetImageResource( Resource.Mipmap.cross );
                            _gameBoardDictionary[gameBoardButtonView.Id] = GameButtonStates.Cross;
                            _nextMove = GameButtonStates.Circle;
                        }
                        else
                        {
                            _symbolGamer = GameButtonStates.Circle;
                            imageButton.SetImageResource( Resource.Mipmap.circle );
                            _gameBoardDictionary[gameBoardButtonView.Id] = GameButtonStates.Circle;
                            _nextMove = GameButtonStates.Cross;
                        }

                        //labelRuch.Text = "Ruch gracza: " + gracz;
                        //pictureBox1.Enabled = false;
                        if (ruchy == 25)
                        {
                            Remis();
                        }
                    }
                    */
                }
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

            eGameStatus = CheckWin(boardButtonPoint);

            return eGameStatus;
        }

        private GameStatus CheckWin(int[] boardButtonPoint)
        {
            GameStatus eWinStatus = CheckHorizontally();

            if( eWinStatus != GameStatus.Win )
            {
                eWinStatus = CheckVertically();

                if( eWinStatus != GameStatus.Win )
                {
                    eWinStatus = CheckCrossing(boardButtonPoint);
                }
            }

            return eWinStatus;

        }

        private GameStatus CheckCrossing(int[] boardButtonPoint)
        {
            GameStatus eWinStatus = GameStatus.Continue;
            int iCounter = 0;

            int[] boardCombinationXArray = { boardButtonPoint[0] - 1,
                                             boardButtonPoint[0] - 2,
                                             boardButtonPoint[0] + 1,
                                             boardButtonPoint[0] + 2 };

            int[] boardCombinationYArray = { boardButtonPoint[1] - 1,
                                             boardButtonPoint[1] - 2,
                                             boardButtonPoint[1] + 1,
                                             boardButtonPoint[1] + 2 };

            Dictionary<int, int> rememberDictionary = new Dictionary<int, int>();

            if (_gameBoardArray[ boardButtonPoint[0], boardButtonPoint[1] ] == _symbolGamer )
            {
                for( int iCounterCombinationX = 0; iCounterCombinationX < boardCombinationXArray.Length; iCounterCombinationX++ )
                {
                    if (boardCombinationXArray[iCounterCombinationX] >= 0 
                        && boardCombinationXArray[iCounterCombinationX] < SIZE_OF_BOARD )
                    {
                        for (int iCounterCombinationY = 0; iCounterCombinationY < boardCombinationYArray.Length; iCounterCombinationY++)
                        {
                            if(boardCombinationYArray[iCounterCombinationY] >= 0 
                                && boardCombinationYArray[iCounterCombinationY] < SIZE_OF_BOARD )
                            {
                                if(_gameBoardArray[ boardCombinationXArray[iCounterCombinationX], boardCombinationYArray[iCounterCombinationY] ] == _symbolGamer 
                                    && !rememberDictionary.ContainsKey(boardCombinationXArray[iCounterCombinationX]))
                                {
                                    rememberDictionary.Add(boardCombinationXArray[iCounterCombinationX], boardCombinationYArray[iCounterCombinationY]);
                                }
                            }
                        }
                    }
                }

                if(rememberDictionary.Count >= 2 )
                {
                    rememberDictionary.Add(boardButtonPoint[0], boardButtonPoint[1]);

                    rememberDictionary = rememberDictionary
                        .OrderBy(key => key.Key)
                        .ToDictionary(c => c.Key, d => d.Value);

                    List<int> rememberDictionaryList = rememberDictionary.Keys.ToList();
                    List<int> rememberDictionaryValueList = rememberDictionary.Values.ToList();
                    int differenceX = 0;
                    int differenceY = 0;

                    for (int i = 1; i < rememberDictionaryList.Count; i++)
                    {
                        differenceX = Math.Abs(rememberDictionaryList[i - 1] - rememberDictionaryList[i]);
                        differenceY = Math.Abs(rememberDictionaryValueList[i - 1] - rememberDictionaryValueList[i]);

                        if (differenceX == 1 && differenceY == 1 )
                        {
                            iCounter++;

                            if (iCounter == 2)
                            {
                                eWinStatus = GameStatus.Win;
                                break;
                            }
                        }
                    }
                }
            }

            return eWinStatus;
        }

        private GameStatus CheckVertically()
        {
            GameStatus eWinStatus = GameStatus.Continue;
            int iCountSameSymbols = 0;

            for( int iIteratorColumn = 0; iIteratorColumn < SIZE_OF_BOARD; iIteratorColumn++ )
            {   
                for( int iIteratorRow = 0; iIteratorRow < SIZE_OF_BOARD; iIteratorRow++ )
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

            for (int iIteratorRow = 0; iIteratorRow < SIZE_OF_BOARD; iIteratorRow++)
            {
                for (int iIteratorColumn = 0; iIteratorColumn < SIZE_OF_BOARD; iIteratorColumn++)
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

            for( int iIteratorX = 0; iIteratorX < SIZE_OF_BOARD; iIteratorX++ )
            {
                for( int iIteratorY = 0; iIteratorY < SIZE_OF_BOARD; iIteratorY++)
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

