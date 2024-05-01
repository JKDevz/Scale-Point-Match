using UnityEngine;
using TMPro; // Using Text Mesh PRO

public class PlayerManager : MonoBehaviour
{
    public TMP_InputField player1NameInput; 
    public TMP_InputField player2NameInput; 
    public PlayerData player1Data;
    public PlayerData player2Data;
    public TMP_Text player1Text; 
    public TMP_Text player2Text; 
    public UnityEngine.UI.Button playButton; 

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
                player1Data.playerColor = Color.red;
                player1Data.playerName = player1NameInput.text;
                player2Data.playerColor = Color.blue;
                player2Data.playerName = player2NameInput.text;
            }
            else
            {
                player1Data.playerColor = Color.blue;
                player1Data.playerName = player2NameInput.text;
                player2Data.playerColor = Color.red;
                player2Data.playerName = player1NameInput.text;
            }

            // Update player text display with names and colors
            player1Text.text = $"{player1Data.playerName} ({(player1IsRed ? "Red" : "Blue")})";
            player2Text.text = $"{player2Data.playerName} ({(player1IsRed ? "Blue" : "Red")})";
        }
        else
        {
            Debug.LogError("One or more UI elements or PlayerData references are not assigned in GameManager");
        }
    }
}