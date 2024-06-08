using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator
{
    public Node GenerateTree(Node rootNode, int depth, PlayerType currentPlayer, GameBoard gameBoard)
    {
        if (depth <= 0 || gameBoard.GetScores().redScore == 0 || gameBoard.GetScores().blueScore == 0)
        {
            // If max depth reached or game over return the current state
            return rootNode;
        }

        List<Vector2> possibleMoves = GetPossibleMoves(currentPlayer, gameBoard);

        foreach (Vector2 move in possibleMoves)
        {
            Node childNode = new Node();
            childNode.movePosition = move;
            childNode.parent = rootNode;
            childNode.isMaximiser = currentPlayer == PlayerType.red; // Red maximizes and blue minimizes
            childNode.gamePhase = rootNode.gamePhase + 1;

            GameBoard simulatedBoard = SimulateMove(move, currentPlayer, gameBoard);
            childNode.simulation = simulatedBoard;

            rootNode.children.Add(childNode);

            GenerateTree(childNode, depth - 1, GetNextPlayer(currentPlayer), simulatedBoard);
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
                if (board[x, y] == 0)
                {
                    possibleMoves.Add(new Vector2(x, y));
                }
            }
        }

        return possibleMoves;
    }

    private PlayerType GetNextPlayer(PlayerType currentPlayer)
    {
        return currentPlayer == PlayerType.red ? PlayerType.blue : PlayerType.red;
    }

    private GameBoard SimulateMove(Vector2 move, PlayerType playerType, GameBoard gameBoard)
    {
        int[,] newBoard = (int[,])gameBoard.GetBoard().Clone();
        GameBoard newGameBoard = new GameBoard(newBoard);

        newGameBoard.MakeMove(move, MoveType.place, playerType);

        return newGameBoard;
    }
}
