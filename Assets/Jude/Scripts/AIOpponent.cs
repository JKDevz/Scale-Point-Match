using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIOpponent : MonoBehaviour
{
    #region VARIABLES

    [Header("Minimax Settings")]
    public int easySearchDepth, mediumSearchDepth, hardSearchDepth;
    public AIMode difficulty;

    [Header("Player References")]
    public Player aiPlayer;

    [Header("AI Settings")]
    public AISettingsSO aiSettings;
    [Range(0, 5)] public float moveWaitMin;
    [Range(0, 5)] public float moveWaitMax;

    public TreeGenerator treeGenerator;
    [HideInInspector] public Node rootNode;

    private int depth;

    private bool myTurn;

    //Reference Leandro's Tree Node

    #endregion

    #region ENABLE/DISABLE

    private void OnEnable()
    {
        GameManager.onMoveMade += PlayerMadeMove;
    }

    private void OnDisable()
    {
        GameManager.onMoveMade -= PlayerMadeMove;
    }

    #endregion

    #region UNITY METHODS

    private void Awake()
    {
        this.enabled = GameManager.Instance.isSinglePlayer;

        aiPlayer = GameManager.Instance.playerTwoData.playerInfo;
        treeGenerator = new TreeGenerator();
        difficulty = aiSettings.aiMode;
        //AI Opponent is always Player 2
        //Of course, being Red or Blue determines who will player first.

        if (difficulty == AIMode.easy)
        {
            depth = easySearchDepth;
        }
        else if (difficulty == AIMode.medium)
        {
            depth = mediumSearchDepth;
        }
        else if (difficulty == AIMode.hard)
        {
            depth = hardSearchDepth;
        }

        if (aiPlayer.colour == PlayerType.red)
        {
            myTurn = true;
        }
        else
        {
            myTurn = false;
        }
    }

    private IEnumerator Start()
    {
        rootNode = treeGenerator.GenerateTree(GameManager.Instance.instanceNode, depth, GameManager.Instance.currentPlayer.colour, GameManager.Instance.gameBoard);

        yield return new WaitForSeconds(1);

        Minimax.DoMinimax(rootNode, GameManager.Instance.currentPlayer.colour == PlayerType.red, depth, 0);

        if (myTurn)
        {
            MakeMove();
        }
    }

    #endregion

    #region METHOD

    public void PlayerMadeMove()
    {
        StartCoroutine(StartMakeMove());
    }

    private IEnumerator StartMakeMove()
    {
        yield return new WaitForSeconds(Random.Range(moveWaitMin, moveWaitMax));

        myTurn = !myTurn;

        if (myTurn)
        {
            rootNode = treeGenerator.GenerateTree(GameManager.Instance.instanceNode, depth, GameManager.Instance.currentPlayer.colour, GameManager.Instance.gameBoard);
            Minimax.DoMinimax(rootNode, GameManager.Instance.currentPlayer.colour == PlayerType.red, depth, 0);

            MakeMove();
        }
    }

    private void MakeMove()
    {
        if (!myTurn)
        {
            return;
        }

        if (difficulty == AIMode.easy)//Make a random move
        {
            //Choose a Random child node
            Vector2 move = GetPossibleMoves()[Random.Range(0, rootNode.children.Count)];

            GameManager.Instance.PlayerClicked("" + move.x + move.y);
        }
        else if (difficulty == AIMode.medium || difficulty == AIMode.hard)
        {
            int bestValue = 0;

            //Choose out of the nodes
            if (aiPlayer.GetColour() == PlayerType.red)
            {
                bestValue = -10;
            }
            else
            {
                bestValue = 10;
            }

            List<int> moves = new List<int>();

            for (int i = 0; i < rootNode.children.Count; i++)
            {
                if (aiPlayer.GetColour() == PlayerType.red && rootNode.children[i].minimaxValue > bestValue)//If AI is the Maximiser, look for the highest value
                {
                    bestValue = rootNode.children[i].minimaxValue;

                    moves.Clear();
                    moves.Add(i);
                }
                else if (aiPlayer.GetColour() == PlayerType.blue && rootNode.children[i].minimaxValue < bestValue)//If AI is the Minimiser, look for the lowest value
                {
                    bestValue = rootNode.children[i].minimaxValue;

                    moves.Clear();
                    moves.Add(i);
                }
                else if (rootNode.children[i].minimaxValue == bestValue)
                {
                    moves.Add(i);
                }
            }

            Vector2 move = rootNode.children[moves[Random.Range(0, moves.Count)]].movePosition;

            GameManager.Instance.PlayerClicked(((int)move.x).ToString() + ((int)move.y).ToString());
        }
    }

    private List<Vector2> GetPossibleMoves()//Gets all possible moves depending on the AI's colour
    {
        List<Vector2> possibleMoves = new List<Vector2>();

        for (int i = 0; i < rootNode.simulation.GetBoard().Length; i++)
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (rootNode.simulation.GetBoard()[x, y] == 0)//If tile is empty, add the coords
                    {
                        possibleMoves.Add(new Vector2(x, y));
                    }
                    else if (aiPlayer.activeBoxes > 1 && aiPlayer.colour == PlayerType.red && GameManager.Instance.gameBoard.GetBoard()[x,y] == 2)//If the AI is red, then include boxes
                    {
                        possibleMoves.Add(new Vector2(x, y));
                    }
                    else if (aiPlayer.activeBoxes > 1 && aiPlayer.colour == PlayerType.blue && GameManager.Instance.gameBoard.GetBoard()[x, y] == 1)
                    {
                        possibleMoves.Add(new Vector2(x, y));
                    }
                }
            }
        }

        return possibleMoves;
    }

    #endregion
}

public enum AIMode
{
    easy,
    medium,
    hard
}
