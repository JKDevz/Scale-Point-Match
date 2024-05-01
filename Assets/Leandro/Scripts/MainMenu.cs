using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void playervsplayer()
    {
        SceneManager.LoadScene(1);

    }

    public void Settings()
    {
        SceneManager.LoadScene(2);
    }

    public void Info()
    {
        SceneManager.LoadScene(3);
    }

    public void Back()
    {
        SceneManager.LoadScene(4);
    }

    public void playvsai()
    {
        SceneManager.LoadScene(5);
    }


}