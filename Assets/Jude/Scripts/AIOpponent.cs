using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIOpponent : MonoBehaviour
{
    #region VARIABLES

    [Header("Minimax Settings")]
    public int mediumSearchDepth, hardSearchDepth;
    public AIMode difficulty;
    [Space]
    [Range(0, 5)] public float moveWaitMin;
    [Range(0, 5)] public float moveWaitMax;

    [Header("Player References")]
    public Player aiPlayer;

    [SerializeField] private Node rootNode;

    private bool myTurn;

    //Reference Leandro's Tree Node

    #endregion

    #region UNITY METHODS

    private void Awake()
    {
        aiPlayer = GameManager.Instance.playerTwoData.playerInfo;
        //AI Opponent is always Player 2
        //Of course, being Red or Blue determines who will player first.
    }

    #endregion

    #region METHOD

    public void TryMakeMove()
    {

    }

    private IEnumerable MakeMove()
    {
        if (!myTurn)
        {
            yield break;
        }

        if (difficulty == AIMode.easy)//Make a random move
        {
            //Choose a Random child node
            Vector2 move = GetPossibleMoves()[Random.Range(0, rootNode.children.Count)];

            yield return new WaitForSeconds(Random.Range(moveWaitMin, moveWaitMax));

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
                else if (aiPlayer.colour == PlayerType.red && rootNode.children[i].minimaxValue < bestValue)//If AI is the Minimiser, look for the lowest value
                {
                    bestValue = rootNode.children[i].minimaxValue;
                    currentIndex = i;
                }
            }

            Vector2 move = rootNode.children[currentIndex].movePosition;

            yield return new WaitForSeconds(Random.Range(moveWaitMin, moveWaitMax));

            GameManager.Instance.PlayerClicked("" + move.x + move.y);
        }

        myTurn = false;
    }

    private void MadeMove()
    {
        //get the new rootNode
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
                    else if (aiPlayer.colour == PlayerType.red && rootNode.simulation.GetBoard()[x, y] == 2 && aiPlayer.activeBoxes > 1)//If the AI is red, then include boxes
                    {
                        possibleMoves.Add(new Vector2(x, y));
                    }
                    else if (aiPlayer.colour == PlayerType.blue && rootNode.simulation.GetBoard()[x, y] == 1 && aiPlayer.activeBoxes > 1)
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
