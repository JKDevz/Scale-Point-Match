using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("SceneHandler")]
    public SceneHandler sceneHandler;

    public void PlayGame()
    {
        sceneHandler.ChangeScene(sceneHandler.actualGame);
    }

    public void QuitGame()
    {
        sceneHandler.QuitGame();
    }
}