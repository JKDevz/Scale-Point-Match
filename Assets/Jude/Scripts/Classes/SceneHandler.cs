using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneHandler
{
    #region VARIABLES

    [Header("Scene Names")]
    public string mainMenu;
    public string actualGame;

    [Header("Transition Settings")]
    [Range(0,10)]
    public float transitionTime;

    #endregion

    #region METHODS

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion
}
