using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator
{
    public Node GenerateTree(Node rootNode, int depth, PlayerType currentPlayer, GameBoard gameBoard)
    {
        if (depth <= 0 || rootNode.gamePhase == GameManager.Instance.totalTurns)
        {
            // If max depth reached or game over return the current state
            return rootNode;
        }

        List<Vector2> possibleMoves = GetPossibleMoves(currentPlayer, gameBoard);

        rootNode.children = new List<Node>();

        foreach (Vector2 move in possibleMoves)
        {
            Node childNode = new Node();
            childNode.movePosition = move;
            childNode.parent = rootNode;
            childNode.isMaximiser = !(currentPlayer == PlayerType.red); // Red maximizes and blue minimizes flip flop
            childNode.gamePhase = rootNode.gamePhase + 1;
            childNode.children = new List<Node>();

            childNode.simulation = SimulateMove(move, currentPlayer, gameBoard);

            childNode = GenerateTree(childNode, depth - 1, GetNextPlayer(currentPlayer), childNode.simulation);

            rootNode.children.Add(childNode);
        }

        return rootNode;
    }

    private List<Vector2> GetPossibleMoves(PlayerType player, GameBoard gameBoard)
    {
        List<Vector2> possibleMoves = new List<Vector2>();

        int[,] board = gameBoard.GetBoard();
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (board[x, y] == 0 || (player == PlayerType.red && board[x, y] == 2) || (player == PlayerType.blue && board[x,y] == 1))
                {
                    possibleMoves.Add(new Vector2(x, y));
                }
            }
        }

        return possibleMoves;
    }

    private PlayerType GetNextPlayer(PlayerType currentPlayer)
    {
        if (currentPlayer == PlayerType.red)
        {
            return PlayerType.blue;
        }
        else
        {
            return PlayerType.red;
        }
    }

    private GameBoard SimulateMove(Vector2 move, PlayerType playerType, GameBoard gameBoard)
    {
        int[,] newBoard = (int[,])gameBoard.GetBoard().Clone();
        GameBoard newGameBoard = new GameBoard(newBoard);

        if (newGameBoard.GetBoard()[(int)move.x, (int)move.y] == 0) //If the space is empty
        {
            newGameBoard.MakeMove(move, MoveType.place, playerType); // Place the box
        }
        else //If the space is not empty
        {
            newGameBoard.MakeMove(move, MoveType.remove, playerType); //Remove the box
        }

        return newGameBoard;
    }
}
