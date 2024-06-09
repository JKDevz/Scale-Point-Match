using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    #region VARIABLES

    public List<Node> children;
    public Node parent;
    public int minimaxValue;
    public bool isMaximiser;
    public Vector2 movePosition = new Vector2() { x = 0, y = 0};
    public GameBoard simulation;
    public int gamePhase;

    #endregion
}
