using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UIHandler
{
    [Header("Canvas References")]
    public Canvas gameCanvas;

    [Header("UI References")]
    public TMP_Text[] quadrantScales;
    
    [Header("Game Stats")]
    public string gameRoundsMessage;
    public TMP_Text gameRounds;
    
    [Header("Player UI")]
    public TMP_Text redScore;
    public TMP_Text blueScore;
    [Space]
    public string turnsLeftMessage;
    public TMP_Text redTurnsLeft;
    public TMP_Text blueTurnsLeft;
    [Space]
    public GameObject redTurnMessage;
    public GameObject blueTurnMessage;

    [Header("Victory UI")]
    public Canvas victoryScreen;
    public GameObject redVictoryMessage;
    public GameObject blueVictoryMessage;
    public GameObject tieVictoryMessage;

    [Header("Button References")]
    public Button[] gameButtons;

    [Header("Tile Settings")]
    public Color emptyTileColour;
    public Color redTileColour;
    public Color blueTileColour;
    [Space]
    public Sprite spriteEmpty;
    public Sprite spriteFilled;

    public UIHandler()
    {

    }

    #region METHODS

    public void InitialiseUI()
    {
        UpdateBoard();
        UpdateScore();
        UpdateScales();
        UpdateGameRound();
        ToggleTurnMessage();
        victoryScreen.enabled = false;
    }

    public void UpdateBoard()
    {
        int i = 0;
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (GameManager.Instance.gameBoard.GetBoard()[x, y] == 0)
                {
                    gameButtons[i].image.color = emptyTileColour;
                    gameButtons[i].image.sprite = spriteEmpty;
                }
                else if (GameManager.Instance.gameBoard.GetBoard()[x, y] == 1)
                {
                    gameButtons[i].image.color = blueTileColour;
                    gameButtons[i].image.sprite = spriteFilled;
                }
                else if (GameManager.Instance.gameBoard.GetBoard()[x, y] == 2)
                {
                    gameButtons[i].image.color = redTileColour;
                    gameButtons[i].image.sprite = spriteFilled;
                }

                i++;
            }
        }
    }

    public void UpdateScales()
    {
        for (int i = 0; i < 4; i++)
        {
            quadrantScales[i].text = GameManager.Instance.gameBoard.GetQuadrants()[i].GetScale().ToString();
        }
    }

    public void ToggleTurnMessage()
    {
        if (GameManager.Instance.currentPlayer.colour == PlayerType.red)
        {
            redTurnMessage.SetActive(true);
            blueTurnMessage.SetActive(false);
        }
        else
        {
            redTurnMessage.SetActive(false);
            blueTurnMessage.SetActive(true);
        }
    }

    public void UpdateScore()
    {
        blueScore.text = GameManager.Instance.GetScores().blueScore.ToString();
        redScore.text = GameManager.Instance.GetScores().redScore.ToString();
    }

    public void UpdateTurnsLeft()
    {
        blueTurnsLeft.text = turnsLeftMessage + (GameManager.Instance.gameLength - GameManager.Instance.bluePlayer.turnsUsed).ToString();
        redTurnsLeft.text = turnsLeftMessage + (GameManager.Instance.gameLength - GameManager.Instance.redPlayer.turnsUsed).ToString();
    }

    public void UpdateGameRound()
    {
        gameRounds.text = gameRoundsMessage + GameManager.Instance.rounds.ToString();
    }

    public void ToggleButtons()
    {
        for (int i = 0; i < gameButtons.Length; i++)
        {
            gameButtons[i].enabled = GameManager.Instance.gameEnded;
        }
    }

    public void DisplayWinner()
    {
        victoryScreen.enabled = true;
        if (GameManager.Instance.winner == GameManager.Instance.redPlayer)
        {
            redVictoryMessage.SetActive(true);
        }
        else if (GameManager.Instance.winner == null)
        {
            tieVictoryMessage.SetActive(true);
        }
        else
        {
            blueVictoryMessage.SetActive(false);
        }
    }

    #endregion
}
