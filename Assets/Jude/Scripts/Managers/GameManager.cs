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
    public bool isSinglePlayer;

    [Header("AI Settings")]
    public AISettingsSO aiSettings;

    [Header("Player References")]
    public PlayerData playerOneData;
    public PlayerData playerTwoData;
    public Player redPlayer;
    public Player bluePlayer;
    [HideInInspector] public Player winner { get; private set; }

    [Header("Scene Handler")]
    public SceneHandler sceneHandler;

    [Header("UI References")]
    public UIHandler uiHandler;
    
    [Header("Game Board")]
    public GameBoard gameBoard;

    public Node instanceNode;

    [Header(">>>>> EXPOSED FOR TESTING")]
    [SerializeField] private int currentPhase;//Increments to match the "phases" of the game, after both players have made a move and now have the same number of turns remaining.
    //The game is considered over once both players have used all their turns.
    [SerializeField] public Player currentPlayer { get; private set; }
    [HideInInspector] public int rounds { get; private set; }
    [HideInInspector] public int totalTurns { get; private set; }
    [HideInInspector] public bool gameEnded { get; private set; }

    #endregion

    #region DELEGATES

    public delegate void OnGameEnd();
    public static event OnGameEnd onGameEnd;

    public delegate void OnMoveMade();
    public static event OnMoveMade onMoveMade;

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

    }

    private void OnDisable()
    {

    }

    #endregion

    #region UNITY METHODS

    private void Awake()
    {
        _instance = FindFirstObjectByType<GameManager>();
        isSinglePlayer = aiSettings.isSinglePlayer;
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
        else if (currentPlayer.activeBoxes > 1 && currentPlayer.colour == PlayerType.red && gameBoard.GetBoard()[x, y] == 2)//If the Box and Player are Red, then remove it
        {
            gameBoard.MakeMove(new Vector2(x, y), MoveType.remove, currentPlayer.colour);
            MadeMove(MoveType.remove);
        }
        else if (currentPlayer.activeBoxes > 1 && currentPlayer.colour == PlayerType.blue && gameBoard.GetBoard()[x, y] == 1)//If the Box and Player are Blue, then remove it
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
        onMoveMade?.Invoke();

        currentPlayer.MadeMove();
        uiHandler.UpdateBoard();
        uiHandler.UpdateScore();
        uiHandler.UpdateScales();
        uiHandler.UpdateTurns();

        if (IsGameOver())
        {
            return;
        }

        SwapPlayer();

        redPlayer.SetActiveBoxes();
        bluePlayer.SetActiveBoxes();

        instanceNode.gamePhase++;
        instanceNode.isMaximiser = !instanceNode.isMaximiser;
        instanceNode.simulation = gameBoard;
        instanceNode.parent = null;
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
        if (redPlayer.turnsUsed >= gameLength && bluePlayer.turnsUsed >= gameLength)
        {
            Debug.Log("Game Over Triggered!");
            GameOver();
            onGameEnd?.Invoke();
            return true;
        }

        return false;
    }

    private void GameOver()
    {
        Debug.Log("Game Over Handled!");

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

        uiHandler.ToggleButtons();
        uiHandler.DisplayWinner();
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
        redPlayer.turnsUsed = 0;
        redPlayer.activeBoxes = 0;

        bluePlayer.turnsMax = gameLength;
        bluePlayer.turnsUsed = 0;
        bluePlayer.activeBoxes = 0;


        gameBoard = new GameBoard(new int[4, 4]);

        rounds = 1;
        totalTurns = gameLength * 2;

        currentPlayer = redPlayer;

        instanceNode = new Node();

        instanceNode.parent = null;
        instanceNode.isMaximiser = currentPlayer.colour == PlayerType.red; // Red maximizes and blue minimizes
        instanceNode.gamePhase = 0;
        instanceNode.simulation = gameBoard;
        instanceNode.children = new List<Node>();

        uiHandler.InitialiseUI();
    }

    #endregion
}

public enum MoveType
{
    place,
    remove,
    none
}
