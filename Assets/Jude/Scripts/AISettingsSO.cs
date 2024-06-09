using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AI Settings", menuName = "AI", order = 0)]
public class AISettingsSO : ScriptableObject
{
    public bool isSinglePlayer;
    public AIMode aiMode;

    public void SetSinglePlayer(bool active)
    {
        isSinglePlayer = active;
    }

    public void SetAIEasy()
    {
        aiMode = AIMode.easy;
    }

    public void SetAIMedium()
    {
        aiMode = AIMode.medium;
    }

    public void SetAIHard()
    {
        aiMode = AIMode.hard;
    }
}
