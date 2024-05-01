using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region VARIABLES
    private static GameManager _instance;

    [Header("Game Settings")]
    public int gameLength;//Total turns per player
    public int maxPlayerBoxes = 9;//Should always be an uneven number

    [Header("Player References")]
    public PlayerData playerOneData;
    public PlayerData playerTwoData;
    [HideInInspector] public Player redPlayer;
    [HideInInspector] public Player bluePlayer;
    [HideInInspector] public Player winner { get; private set; }

    [Header("Scene Handler")]
    public SceneHandler sceneHandler;

    [Header("UI References")]
    public UIHandler uiHandler;
    
    [Header("Game Board")]
    public GameBoard gameBoard;

    [Header(">>>>> EXPOSED FOR TESTING")]
    [SerializeField] private int currentPhase;//Increments to match the "phases" of the game, after both players have made a move and now have the same number of turns remaining.
    //The game is considered over once both players have used all their turns.
    [SerializeField] public Player currentPlayer { get; private set; }
    [HideInInspector] public int rounds { get; private set; }
    [HideInInspector] public bool gameEnded { get; private set; }

    #endregion

    #region DELEGATES

    public delegate void OnGameEnd();
    public static event OnGameEnd onGameEnd;

    #endregion

    #region SINGLETON STUFF

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();

                if (_instance == null)
                {
                    GameObject singleton = new GameObject("GameManager");
                    _instance = singleton.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
        private set { }
    }

    #endregion

    #region DISABLE/ENABLE

    private void OnEnable()
    {
        onGameEnd += uiHandler.ToggleButtons;
        onGameEnd += uiHandler.DisplayWinner;
        onGameEnd += GameOver;
    }

    private void OnDisable()
    {
        onGameEnd -= uiHandler.ToggleButtons;
        onGameEnd -= uiHandler.DisplayWinner;
        onGameEnd -= GameOver;
    }

    #endregion

    #region UNITY METHODS

    private void Awake()
    {
        _instance = FindFirstObjectByType<GameManager>();
        InitialiseGame();
    }

    #endregion

    #region METHODS

    public void PlayerClicked(string pos)
    {
        int x = (int)char.GetNumericValue(pos[0]);
        int y = (int)char.GetNumericValue(pos[1]);

        //If the move is "outside" of the game board, then don't do anything
        if (x > gameBoard.GetBoard().GetLength(0) || x < 0 || y > gameBoard.GetBoard().GetLength(1) || y < 0)
        {
            return;
        }

        if (currentPlayer.activeBoxes < maxPlayerBoxes && gameBoard.GetBoard()[x, y] == 0)//If the tile is empty, place a box
        {
            gameBoard.MakeMove(new Vector2(x,y), MoveType.place, currentPlayer.colour);
            MadeMove(MoveType.place);
        }
        else if (currentPlayer.colour == PlayerType.red && gameBoard.GetBoard()[x, y] == 2)//If the Box and Player are Red, then remove it
        {
            gameBoard.MakeMove(new Vector2(x, y), MoveType.remove, currentPlayer.colour);
            MadeMove(MoveType.remove);
        }
        else if (currentPlayer.colour == PlayerType.blue && gameBoard.GetBoard()[x, y] == 1)//If the Box and Player are Blue, then remove it
        {
            gameBoard.MakeMove(new Vector2(x, y), MoveType.remove, currentPlayer.colour);
            MadeMove(MoveType.remove);
        }
    }

    public (int redScore, int blueScore) GetScores()
    {
        int blueScore = 0;
        int redScore = 0;

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (gameBoard.GetBoard()[x,y] == 1)
                {
                    blueScore++;
                }
                else if (gameBoard.GetBoard()[x,y] == 2)
                {
                    redScore++;
                }
            }
        }

        return (redScore, blueScore);
    }

    private void MadeMove(MoveType moveType)
    {
        currentPlayer.MadeMove();
        uiHandler.UpdateBoard();
        uiHandler.UpdateScore();
        uiHandler.UpdateScales();
        uiHandler.UpdateTurnsLeft();

        if (IsGameOver())
        {
            return;
        }

        SwapPlayer();
    }

    private void SwapPlayer()
    {
        if (currentPlayer == redPlayer)
        {
            currentPlayer = bluePlayer;
        }
        else
        {
            currentPlayer = redPlayer;
            rounds++;
            uiHandler.UpdateGameRound();
        }

        uiHandler.ToggleTurnMessage();
    }

    private bool IsGameOver()
    {
        if (redPlayer.turnsUsed == gameLength && bluePlayer.turnsUsed == gameLength)
        {
            onGameEnd?.Invoke();
            return true;
        }

        return false;
    }

    private void GameOver()
    {
        int blueScore = GetScores().blueScore;
        int redScore = GetScores().redScore;

        if (redScore > blueScore)
        {
            winner = redPlayer;
        }
        else if (redScore == blueScore)
        {
            winner = null;
        }
        else
        {
            winner = bluePlayer;
        }
    }

    private void InitialiseGame()
    {
        if (playerOneData.playerInfo.colour == PlayerType.red)
        {
            redPlayer = playerOneData.playerInfo;
            bluePlayer = playerTwoData.playerInfo;
        }
        else
        {
            bluePlayer = playerOneData.playerInfo;
            redPlayer = playerTwoData.playerInfo;
        }

        redPlayer.turnsMax = gameLength;
        bluePlayer.turnsMax = gameLength;

        gameBoard = new GameBoard(new int[4, 4]);

        rounds = 1;
        currentPlayer = redPlayer;

        uiHandler.InitialiseUI();
    }

    public void RestartGame()
    {
        sceneHandler.ChangeScene(sceneHandler.actualGame);
    }

    #endregion
}

public enum MoveType
{
    place,
    remove
}
