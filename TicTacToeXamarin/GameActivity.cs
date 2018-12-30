using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Interop;
using TicTacToeXamarin.Game;

namespace TicTacToeXamarin
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameActivity : AppCompatActivity
    {
        const int REQUEST_ENABLE_BT = 3;
        const char COORDINATE_MESSAGE_SEPARATOR = ';';
        private const int SIZE_OF_BOARD_VALUE = 5;
        private const int COMPLETE_TILE_TO_WIN_VALUE = 3;
        private TableLayout _gameBoardTableLayout;
        private Dictionary<int, GameButtonStates> _gameBoardDictionary;
        private GameButtonStates[,] _gameBoardArray;
        private GameButtonStates _currentSymbolGamer;
        private GameButtonStates _yourSymbolGame;
        private TextView _circleTextView;
        private TextView _crossTextView;
        private TextView _nextMoveTextView;
        private TextView _resultTextView;
        int iScoreOfCirclePlayer;
        int iScoreOfCrossPlayer;
        int iAmountOfMoves;

        BluetoothDeviceInfo _oponentDeviceInfo;
        BluetoothAdapter _bluetoothAdapter;
        BluetoothService _chatService;
        DiscoverableModeReceiver _receiver;
        GameMessageHandler _gameMessageHandler;

        public GameActivity()
        {
            _currentSymbolGamer = GameButtonStates.Circle;
            _yourSymbolGame = _currentSymbolGamer;
            iScoreOfCirclePlayer = 0;
            iScoreOfCrossPlayer = 0;
            iAmountOfMoves = 0;
            _oponentDeviceInfo = null;
            _chatService = null;
            _bluetoothAdapter = null;
        }

        protected override void OnCreate( Bundle savedInstanceState )
        {
            base.OnCreate(savedInstanceState);
            _oponentDeviceInfo = GameTools.bluetoothManager.GetBluetoothDeviceOpponent();
            SetContentView(Resource.Layout.game_activity);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            InitTextViews();
            InitBoard();
            
            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            _receiver = new DiscoverableModeReceiver();

            if( _bluetoothAdapter == null )
            {
                Toast.MakeText(Application.Context, "Bluetooth is not available.", ToastLength.Long).Show();
                FinishAndRemoveTask();
            }
        }

        public string GetNameDevice()
        {
            return _bluetoothAdapter.Name;
        }

        protected override void OnStart()
        {
            base.OnStart();

            if( !_bluetoothAdapter.IsEnabled )
            {
                var enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableIntent, REQUEST_ENABLE_BT);
            }
            else if( _chatService == null )
            {
                _gameMessageHandler = new GameMessageHandler(this);
                SetupChat();
                ConnectDevice( true );
            }

            var filter = new IntentFilter( BluetoothAdapter.ActionScanModeChanged );
            RegisterReceiver( _receiver, filter );
        }

        public void SetPlayerMessage()
        {
            //SendMessage(GetFormatedCoordinateString(new int[] { (int)_yourSymbolGame, 0, 0 }), GameMessageType.SetPlayer);
            _yourSymbolGame = GameButtonStates.Cross;
        }

        void SetupChat()
        {
            _chatService = new BluetoothService( _gameMessageHandler );
        }

        protected override void OnResume()
        {
            base.OnResume();
            if( _chatService != null )
            {
                if( _chatService.GetState() == TicTacToeXamarin.BluetoothService.STATE_NONE)
                {
                    _chatService.Start();
                }
            }
        }

        void ConnectDevice( bool bIsSecure )
        {
            string macAddressString = _oponentDeviceInfo.macDeviceString;
            BluetoothDevice opponentBluetoothDevice = _bluetoothAdapter.GetRemoteDevice( macAddressString );
            _chatService.Connect( opponentBluetoothDevice, bIsSecure );
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterReceiver(_receiver);

            if ( _chatService != null )
            {
                _chatService.Stop();
            }
        }

        private void InitTextViews()
        {
            _circleTextView = FindViewById<TextView>( Resource.Id.circleTextView );
            _crossTextView = FindViewById<TextView>( Resource.Id.crossTextView );
            _nextMoveTextView = FindViewById<TextView>( Resource.Id.nextMoveTextView );
            _resultTextView = FindViewById<TextView>(Resource.Id.resultTextView);

            _circleTextView.SetBackgroundColor(Android.Graphics.Color.Firebrick);
            _crossTextView.SetBackgroundColor(Android.Graphics.Color.Coral);
            _nextMoveTextView.SetBackgroundColor(Android.Graphics.Color.Azure);

            _circleTextView.Text = GetScoreText(GameButtonStates.Circle, iScoreOfCirclePlayer);
            _crossTextView.Text = GetScoreText(GameButtonStates.Cross, iScoreOfCrossPlayer);
            _nextMoveTextView.Text = GetNextMovePlayerName();
            _resultTextView.Visibility = ViewStates.Invisible;
        }

        private string GetNextMovePlayerName()
        {
            string sNextPlayerString = "Nieznany!";

            switch( _currentSymbolGamer )
            {
                case GameButtonStates.Circle:
                    sNextPlayerString = "Kółko";
                    break;
                case GameButtonStates.Cross:
                    sNextPlayerString = "Krzyżyk";
                    break;
                case GameButtonStates.Standard:
                    sNextPlayerString = "Standard";
                    break;
                default:
                    break;
            }

            if( _currentSymbolGamer != _yourSymbolGame )
            {
                sNextPlayerString += " (Ty)";
            }

            return Convert.ToString( "Następny ruch: " + sNextPlayerString );
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
            if( _currentSymbolGamer == GameButtonStates.Circle )
            {
                iScoreOfCirclePlayer++;
                _circleTextView.Text = GetScoreText( GameButtonStates.Circle, iScoreOfCirclePlayer );
            }
            else
            {
                iScoreOfCrossPlayer++;
                _crossTextView.Text = GetScoreText(GameButtonStates.Cross, iScoreOfCrossPlayer);
            }

            _resultTextView.Text = "Wygrał: " + GetWinnerNameText();
            _resultTextView.Visibility = ViewStates.Visible;

            using ( var alertDialogBuilder = new Android.Support.V7.App.AlertDialog.Builder( this ) )
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

        private string GetWinnerNameText()
        {
            string sPlayerName = "Nieznany";

            switch (_currentSymbolGamer)
            {
                case GameButtonStates.Circle:
                    sPlayerName = "Kółko";
                    break;
                case GameButtonStates.Cross:
                    sPlayerName = "Krzyżyk";
                    break;
                case GameButtonStates.Standard:
                    sPlayerName = "Standard";
                    break;
                default:
                    break;
            }

            return Convert.ToString(sPlayerName);
        }

        private string GetScoreText( GameButtonStates eGamePlayer, int iScoreOfCirclePlayer)
        {
            string sPlayerName = "Nieznany";

            switch( eGamePlayer )
            {
                case GameButtonStates.Circle:
                    sPlayerName = "Kółko";
                    break;
                case GameButtonStates.Cross:
                    sPlayerName = "Krzyżyk";
                    break;
                case GameButtonStates.Standard:
                    sPlayerName = "Standard";
                    break;
                default:
                    break;
            }

            return Convert.ToString(sPlayerName + ": " + iScoreOfCirclePlayer + " pkt");
        }

        private void OkAction( object sender, DialogClickEventArgs e )
        {
            ClearGameBoard();

            if ( _currentSymbolGamer == GameButtonStates.Cross )
            {
                _currentSymbolGamer = GameButtonStates.Circle;
            }
            else
            {
                _currentSymbolGamer = GameButtonStates.Cross;
            }

            SetSymbolNextGamer();
            _resultTextView.Visibility = ViewStates.Invisible;

            Toast.MakeText( ApplicationContext, GetScoreText(GameButtonStates.Circle, iScoreOfCirclePlayer) 
                + "\n" + GetScoreText(GameButtonStates.Cross, iScoreOfCrossPlayer), ToastLength.Short ).Show();
        }

      private void CancelAction( object sender, DialogClickEventArgs e )
      {
            ClearGameBoard();
            _resultTextView.Visibility = ViewStates.Invisible;
            FinishAffinity();
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

        [Export("OnGameExit")]
        public void OnGameExit( View gameBoardButtonView )
        {
            FinishAffinity();
        }

        [Export("OnGameBoardButtonClick")]
        public void OnGameBoardButtonClick( View gameBoardButtonView )
        {
            int[] boardButtonPoint = new int[3];

            if ( gameBoardButtonView != null 
                && ( gameBoardButtonView is ImageButton )
                && _gameBoardDictionary.ContainsKey( gameBoardButtonView.Id ) )
            {
                if( _currentSymbolGamer != _yourSymbolGame )
                {
                    boardButtonPoint = GetButtonCoordinatePointByID(gameBoardButtonView.Id);
                    UpdateGameBoard(boardButtonPoint);
                    InformOpponentAboutMove(boardButtonPoint);
                }
                else
                {
                    Toast.MakeText( Application.Context, "Poczekaj na swoją kolej!", ToastLength.Long ).Show();
                }
            }
        }

        private void UpdateGameBoard( int[] boardButtonPoint )
        {
            ImageButton imageButton = null;
            GameButtonStates gameButtonState = GameButtonStates.Standard;

            gameButtonState = _gameBoardDictionary[boardButtonPoint[2]];
            imageButton = FindViewById<ImageButton>(boardButtonPoint[2]);

            if( imageButton != null
                && gameButtonState == GameButtonStates.Standard)
            {
                iAmountOfMoves++;
                _gameBoardArray[boardButtonPoint[0], boardButtonPoint[1]] = _currentSymbolGamer;

                switch (_currentSymbolGamer)
                {
                    case GameButtonStates.Circle:
                        imageButton.SetImageResource(Resource.Mipmap.circle);
                        _gameBoardDictionary[boardButtonPoint[2]] = GameButtonStates.Circle;
                        break;
                    case GameButtonStates.Cross:
                        imageButton.SetImageResource(Resource.Mipmap.cross);
                        _gameBoardDictionary[boardButtonPoint[2]] = GameButtonStates.Cross;
                        break;
                    default:
                        break;
                }

                switch (GetGameStatus(boardButtonPoint))
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

        private void InformOpponentAboutMove( int[] boardButtonPoint )
        {
            SendMessage( GetFormatedCoordinateString( boardButtonPoint ), GameMessageType.UpdateView );
        }

        private string GetFormatedCoordinateString( int[] boardButtonPoint )
        {
            string messageString = string.Empty;

            for (int iCoordinatePointIndex = 0; iCoordinatePointIndex < boardButtonPoint.Length; iCoordinatePointIndex++)
            {
                messageString += Convert.ToString(boardButtonPoint[iCoordinatePointIndex]) + COORDINATE_MESSAGE_SEPARATOR;
            }

            return messageString;
        }

        public void RemoteUpdateGame( string readMessage )
        {
            switch( GetGameMessageType( readMessage ) )
            {
                case GameMessageType.UpdateView:

                    int[] encodedCoordinateArray = GetEncodedCoordinateIntArray(readMessage);

                    if ( _gameBoardDictionary.ContainsKey( encodedCoordinateArray[2] ) )
                    {
                        UpdateGameBoard( encodedCoordinateArray );
                    }

                    break;
                case GameMessageType.SetPlayer:
                    //SetPlayerSymbol( readMessage );
                    break;
                case GameMessageType.Unknown:
                    break;
                default:
                    break;
            }
        }

        private void SetPlayerSymbol( string readMessage )
        {
            GameButtonStates gameButtonStates = GameButtonStates.Standard;
            int valueIntOfPlayerState = -1;

            string[] encodedPlayerSymbolSplitArrayString = readMessage.Split(COORDINATE_MESSAGE_SEPARATOR);

            if( int.TryParse( encodedPlayerSymbolSplitArrayString[0], out valueIntOfPlayerState ) )
            {
                gameButtonStates = (GameButtonStates)valueIntOfPlayerState;

                switch( gameButtonStates )
                {
                    case GameButtonStates.Circle:
                        _yourSymbolGame = GameButtonStates.Cross;
                        break;
                    case GameButtonStates.Cross:
                        _yourSymbolGame = GameButtonStates.Circle;
                        break;
                    case GameButtonStates.Standard:
                        _yourSymbolGame = GameButtonStates.Cross;
                        break;
                    default:
                        break;
                }

            }
        }

        private GameMessageType GetGameMessageType( string readMessage )
        {
            GameMessageType gameMessageType = GameMessageType.Unknown;
            int valueIntOfGameMessageType = -1;

            string[] encodedCoordinateSpliArrayString = readMessage.Split(COORDINATE_MESSAGE_SEPARATOR);

            if( int.TryParse(encodedCoordinateSpliArrayString[3], out valueIntOfGameMessageType ) )
            {
                gameMessageType = (GameMessageType) valueIntOfGameMessageType;
            }

            return gameMessageType;
        }

        private int[] GetEncodedCoordinateIntArray( string encodedCoordinateString )
        {
            int[] boardButtonPoint = new int[ 3 ];

            string[] encodedCoordinateSpliArrayString = encodedCoordinateString.Split( COORDINATE_MESSAGE_SEPARATOR );

            for( int iIndex = 0; iIndex < boardButtonPoint.Length; iIndex++ )
            {
                boardButtonPoint[iIndex] = int.Parse( encodedCoordinateSpliArrayString[iIndex] );
            }

            return boardButtonPoint;
        }

        public void SendMessage( String messageString, GameMessageType eGameMessageType )
        {
            if( _chatService.GetState() == TicTacToeXamarin.BluetoothService.STATE_CONNECTED
                && messageString.Length > 0 )
            {
                messageString += Convert.ToString( (int)eGameMessageType ) + COORDINATE_MESSAGE_SEPARATOR;

                byte[] bytes = Encoding.ASCII.GetBytes( messageString );
                _chatService.Write( bytes );
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
            switch(_currentSymbolGamer)
            {
                case GameButtonStates.Circle:
                    _currentSymbolGamer = GameButtonStates.Cross;
                    break;
                case GameButtonStates.Cross:
                    _currentSymbolGamer = GameButtonStates.Circle;
                    break;
                default:
                    break;
            }

            _nextMoveTextView.Text = GetNextMovePlayerName();
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

            if( _gameBoardArray[ boardButtonPoint[0], boardButtonPoint[1] ] == _currentSymbolGamer )
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
                            if (_gameBoardArray[boardCombinationXArray[iCounterCombinationX], boardCombinationYArray[iCounterCombinationY]] == _currentSymbolGamer)
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
                    if( _gameBoardArray[ iIteratorRow, iIteratorColumn ] == _currentSymbolGamer )
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
                    if (_gameBoardArray[iIteratorRow, iIteratorColumn] == _currentSymbolGamer)
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
            int[] boardButtonCoordinatePointArray = new int[ 3 ];

            for( int iIteratorX = 0; iIteratorX < SIZE_OF_BOARD_VALUE; iIteratorX++ )
            {
                for( int iIteratorY = 0; iIteratorY < SIZE_OF_BOARD_VALUE; iIteratorY++)
                {
                    if( iButtonId == iCounterValue)
                    {
                        boardButtonCoordinatePointArray[ 0 ] = iIteratorX;
                        boardButtonCoordinatePointArray[ 1 ] = iIteratorY;
                        boardButtonCoordinatePointArray[ 2 ] = iButtonId;
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

