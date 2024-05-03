using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    #region VARIABLES

    public PlayerType colour;
    public string name;
    [Space]
    public int turnsMax;
    public int turnsUsed;
    public int activeBoxes;

    #endregion

    #region CONSTRUCTOR

    public Player(PlayerType colour, int turnsMax, int turnsUsed)
    {
        this.colour = colour;
        this.turnsMax = turnsMax;
        this.turnsUsed = turnsUsed;
        activeBoxes = 0;
    }

    #endregion

    #region METHODS

    public void MadeMove()
    {
        SetActiveBoxes();

        turnsUsed++;
    }

    public void SetActiveBoxes()
    {
        if (colour == PlayerType.red)
        {
            activeBoxes = GameManager.Instance.GetScores().redScore;
        }
        else
        {
            activeBoxes = GameManager.Instance.GetScores().blueScore;
        }
    }

    public void RetractMove()
    {
        turnsUsed--;
    }

    public int GetTurnsUsed()
    {
        return turnsUsed;
    }

    public int GetMaxTurns()
    {
        return turnsMax;
    }

    public PlayerType GetColour()
    {
        return colour;
    }

    #endregion
}

public enum PlayerType
{
    red,
    blue
}
