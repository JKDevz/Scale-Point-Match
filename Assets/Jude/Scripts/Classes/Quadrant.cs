using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quadrant
{
    #region VARIABLES

    [SerializeField] protected GameBoard parent;
    [SerializeField] protected int[,] area;
    [SerializeField] protected int scale;
    [SerializeField] protected int prevScale;

    #endregion

    #region CONSTRUCTOR

    public Quadrant(GameBoard parent, int[,] area, int scale)
    {
        this.parent = parent;
        this.area = area;
        this.scale = scale;
    }

    #endregion

    #region METHODS

    public int[,] GetArea()
    {
        return area;
    }

    public void UpdateScale()
    {
        prevScale = scale;
        scale = 0;//Reset scale

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                if (area[x,y] != 0)
                {
                    scale++;//Increment scale for every Box in the area
                }
            }
        }

        Debug.Log(area[0,0] + " | " + area[1, 0] + " | " + area[0, 1] + " | " + area[1, 1]);
    }

    public int GetScale()
    {
        return scale;
    }

    public int GetLastScale()
    {
        return prevScale;
    }

    public void ModifiyScale(int amount, MathMethod method)
    {
        if (method == MathMethod.increment)
        {
            scale += amount;
        }
        else if (method == MathMethod.decrement)
        {
            scale -= amount;
        }

        if (scale < 0)
        {
            scale = 0;
        }
        else if (scale > area.Length)
        {
            scale = area.Length;
        }
    }

    #endregion
}

public enum MathMethod
{
    increment,
    decrement,
    multiply,
    divide
}
