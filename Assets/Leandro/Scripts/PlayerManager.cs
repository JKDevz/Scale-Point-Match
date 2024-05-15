using UnityEngine;
using TMPro; // Using Text Mesh PRO

public class PlayerManager : MonoBehaviour
{
    [Header("Player Data")]
    public PlayerData player1Data;
    public PlayerData player2Data;

    [Header("2 Player References")]
    public TMP_InputField player1NameInput; 
    public TMP_InputField player2NameInput; 

    public TMP_Text player1Text; 
    public TMP_Text player2Text; 
    public UnityEngine.UI.Button playButton; 

    [Header("1 Player References")]
    public TMP_InputField singlePlayerNameInput;
    public TMP_Text singlePlayerNameText;
    public TMP_Text aiNameText;

    private void Start()
    {
        
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayButtonClick);
        }
        else
        {
            Debug.LogError("Play Button not assigned in PlayerManager.");
        }
    }

    private void OnPlayButtonClick()
    {
       
        if (player1NameInput != null && player2NameInput != null &&
            player1Data != null && player2Data != null &&
            player1Text != null && player2Text != null)
        {
            // Randomly pick color
            bool player1IsRed = Random.Range(0, 2) == 0;

            // Assign player data based on  input names
            if (player1IsRed)
            {
                player1Data.playerInfo.colour = PlayerType.red;
                player1Data.playerInfo.name = player2NameInput.text;
                player2Data.playerInfo.colour = PlayerType.blue;
                player2Data.playerInfo.name = player1NameInput.text;
            }
            else
            {
                player1Data.playerInfo.colour = PlayerType.blue;
                player1Data.playerInfo.name = player2NameInput.text;
                player2Data.playerInfo.colour = PlayerType.red;
                player2Data.playerInfo.name = player1NameInput.text;
            }

            // Update player text display with names and colors
            player1Text.text = $"{player1Data.playerInfo.name} ({(player1IsRed ? "Red" : "Blue")})";
            player2Text.text = $"{player2Data.playerInfo.name} ({(player1IsRed ? "Blue" : "Red")})";
        }
        else
        {
            Debug.LogError("One or more UI elements or PlayerData references are not assigned in GameManager");
        }
    }
}