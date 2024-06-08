using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIOpponent : MonoBehaviour
{
    #region VARIABLES

    [Header("Minimax Settings")]
    public int mediumSearchDepth, hardSearchDepth;
    public AIMode difficulty;

    [Header("Player References")]
    public Player aiPlayer;

    [Header("AI Settings")]
    public AISettingsSO aiSettings;
    [Range(0, 5)] public float moveWaitMin;
    [Range(0, 5)] public float moveWaitMax;

    public TreeGenerator treeGenerator;
    public Node rootNode;

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
        aiPlayer = GameManager.Instance.playerTwoData.playerInfo;
        treeGenerator = new TreeGenerator();
        difficulty = aiSettings.aiMode;
        //AI Opponent is always Player 2
        //Of course, being Red or Blue determines who will player first.

        if (difficulty == AIMode.medium)
        {
            depth = mediumSearchDepth;
        }
        else if (difficulty == AIMode.hard)
        {
            depth = hardSearchDepth;
        }

        this.enabled = !GameManager.Instance.isSinglePlayer;

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

        Debug.Log(rootNode.children.Count);

        yield return new WaitForSeconds(1);

        Minimax.DoMinimax(rootNode, GameManager.Instance.currentPlayer.colour == PlayerType.red, depth, 0);

        if (myTurn)
        {
            Debug.Log("My Turn! Making a Move!");
            MakeMove();
        }
    }

    #endregion

    #region METHOD

    public void PlayerMadeMove()
    {
        Debug.Log("PLayer Mode Move");
        rootNode = treeGenerator.GenerateTree(GameManager.Instance.instanceNode, depth, GameManager.Instance.currentPlayer.colour, GameManager.Instance.gameBoard);
        Minimax.DoMinimax(rootNode, GameManager.Instance.currentPlayer.colour == PlayerType.red, depth, 0);

        myTurn = !myTurn;

        if (myTurn)
        {
            Debug.Log("My Turn! Making a Move!");
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
            Vector2 move = GetPossibleMoves()[Random.Range(0, GameManager.Instance.instanceNode.children.Count)];

            GameManager.Instance.PlayerClicked("" + move.x + move.y);
        }
        else if (difficulty == AIMode.medium || difficulty == AIMode.hard)
        {
            //Choose out of the nodes
            int bestValue = 0;
            int currentIndex = 0;

            for (int i = 0; i < rootNode.children.Count; i++)
            {
                if (aiPlayer.colour == PlayerType.red && rootNode.children[i].minimaxValue > bestValue)//If AI is the Maximiser, look for the highest value
                {
                    bestValue = rootNode.children[i].minimaxValue;
                    currentIndex = i;
                }
                else if (aiPlayer.colour == PlayerType.blue && rootNode.children[i].minimaxValue < bestValue)//If AI is the Minimiser, look for the lowest value
                {
                    bestValue = rootNode.children[i].minimaxValue;
                    currentIndex = i;
                }
            }

            Vector2 move = rootNode.children[currentIndex].movePosition;

            Debug.Log(((int)move.x).ToString() + ((int)move.y).ToString());

            GameManager.Instance.PlayerClicked(((int)move.x).ToString() + ((int)move.y).ToString());
        }
    }

    private void MadeMove()
    {
        //get the new rootNode
    }

    private List<Vector2> GetPossibleMoves()//Gets all possible moves depending on the AI's colour
    {
        List<Vector2> possibleMoves = new List<Vector2>();

        for (int i = 0; i < GameManager.Instance.instanceNode.simulation.GetBoard().Length; i++)
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (GameManager.Instance.instanceNode.simulation.GetBoard()[x, y] == 0)//If tile is empty, add the coords
                    {
                        possibleMoves.Add(new Vector2(x, y));
                    }
                    else if (aiPlayer.colour == PlayerType.red && GameManager.Instance.instanceNode.simulation.GetBoard()[x, y] == 2 && aiPlayer.activeBoxes > 1)//If the AI is red, then include boxes
                    {
                        possibleMoves.Add(new Vector2(x, y));
                    }
                    else if (aiPlayer.colour == PlayerType.blue && GameManager.Instance.instanceNode.simulation.GetBoard()[x, y] == 1 && aiPlayer.activeBoxes > 1)
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
