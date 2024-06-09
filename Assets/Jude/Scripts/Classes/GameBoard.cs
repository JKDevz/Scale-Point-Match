using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameBoard
{
    #region VARIABLES

    [Header("Game Board")]
    [SerializeField] private int[,] gameBoard;
    [SerializeField] private Quadrant[] quadrants;

    //Numbering convention
    //0 = empty tile
    //1 = blue box
    //2 = red box

    #endregion

    #region EVENTS

    public delegate void OnEventScale();
    public static event OnEventScale onEventScale;

    #endregion

    #region CONSTRUCTOR

    public GameBoard(int[,] gameBoard)
    {
        this.gameBoard = gameBoard;
        quadrants = new Quadrant[4]
        {
            new Quadrant(this, new int[2, 2], 0),
            new Quadrant(this, new int[2, 2], 0),
            new Quadrant(this, new int[2, 2], 0),
            new Quadrant(this, new int[2, 2], 0)
        };

        UpdateQuadrants();
        CheckScales();
    }

    #endregion

    #region METHODS

    public int[,] GetBoard()
    {
        return gameBoard;
    }

    public Quadrant[] GetQuadrants()
    {
        return quadrants;
    }

    public (int redScore, int blueScore) GetScores()
    {
        int blueScore = 0;
        int redScore = 0;

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (gameBoard[x, y] == 1)
                {
                    blueScore++;
                }
                else if (gameBoard[x, y] == 2)
                {
                    redScore++;
                }
            }
        }

        return (redScore, blueScore);
    }

    public void MakeMove(Vector2 position, MoveType moveType, PlayerType playerType)
    {
        if (playerType == PlayerType.red)
        {
            if (moveType == MoveType.place)
            {
                gameBoard[(int)position.x, (int)position.y] = 2;
            }
            else if (moveType == MoveType.remove)
            {
                gameBoard[(int)position.x, (int)position.y] = 0;
            }
        }
        else if (playerType == PlayerType.blue)
        {
            if (moveType == MoveType.place)
            {
                gameBoard[(int)position.x, (int)position.y] = 1;
            }
            else if (moveType == MoveType.remove)
            {
                gameBoard[(int)position.x, (int)position.y] = 0;
            }
        }

        UpdateQuadrants();
        CheckScales();
    }

    public void UpdateQuadrants()
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (x < 2 && y < 2)
                {
                    // x-axis /\|\/
                    //[][] X X
                    //[][] X X
                    // X X X X
                    // X X X X
                    ////////// y-axis </>

                    quadrants[0].GetArea()[x, y] = gameBoard[x, y];
                }

                if (x >= 2 && y < 2)
                {
                    // x-axis /\|\/
                    // X X X X
                    // X X X X
                    //[][] X X
                    //[][] X X
                    ////////// y-axis </>

                    quadrants[1].GetArea()[x - 2, y] = gameBoard[x, y];
                }

                if (x < 2 && y >= 2)
                {
                    // x-axis /\|\/
                    // X X [][]
                    // X X [][]
                    // X X X X
                    // X X X X
                    ////////// y-axis </>

                    quadrants[2].GetArea()[x, y - 2] = gameBoard[x, y];
                }

                if (x >= 2 && y >= 2)
                {
                    // x-axis /\|\/
                    // X X X X
                    // X X X X
                    // X X [][]
                    // X X [][]
                    ////////// y-axis </>

                    quadrants[3].GetArea()[x - 2, y - 2] = gameBoard[x, y];
                }
            }
        }

        for (int i = 0; i < quadrants.Length; i++)
        {
            quadrants[i].UpdateScale();
        }
    }

    public void CheckScales()
    {
        //Check if 2 quadrants have the same scale
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (j == i)
                {
                    continue;
                }

                if (quadrants[i].GetScale() == quadrants[j].GetScale())
                {
                    if (EventScaleCheck(quadrants[i]))
                    {
                        HandleEventScale(quadrants[i].GetScale());
                        return;
                    }
                }
            }
        }
    }

    public bool EventScaleCheck(Quadrant quadrant)
    {
        if (quadrant.GetLastScale() != quadrant.GetScale())
        {
            return true;
        }

        return false;
    }

    private void HandleEventScale(int eventScale)
    {
        bool[] quadrantMatches = new bool[4] { true, true, true, true};//Consider every Quadrant to have a matching scale

        for (int i = 0; i < 4; i++)
        {
            if (quadrants[i].GetScale() != eventScale)//Flag a quadrant to be cleared if its scale doesn't match
            {
                quadrantMatches[i] = false;
            }
        }

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (!quadrantMatches[0] && x < 2 && y < 2)
                {
                    // x-axis /\|\/
                    //[][] X X
                    //[][] X X
                    // X X X X
                    // X X X X
                    ////////// y-axis </>

                    gameBoard[x, y] = 0;
                }

                if (!quadrantMatches[1] && x >= 2 && y < 2)
                {
                    // x-axis /\|\/
                    // X X X X
                    // X X X X
                    //[][] X X
                    //[][] X X
                    ////////// y-axis </>

                    gameBoard[x, y] = 0;
                }

                if (!quadrantMatches[2] && x < 2 && y >= 2)
                {
                    // x-axis /\|\/
                    // X X [][]
                    // X X [][]
                    // X X X X
                    // X X X X
                    ////////// y-axis </>

                    gameBoard[x, y] = 0;
                }

                if (!quadrantMatches[3] && x >= 2 && y >= 2)
                {
                    // x-axis /\|\/
                    // X X X X
                    // X X X X
                    // X X [][]
                    // X X [][]
                    ////////// y-axis </>

                    gameBoard[x, y] = 0;
                }
            }
        }

        UpdateQuadrants();
    }

    #endregion
}
