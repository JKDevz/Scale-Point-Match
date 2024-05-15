using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimax
{
    public int minimax(Node node, bool isMaximiser, int depth, int score)
    {
        if (depth == 0)
        {
            return UtilityFunction(node);
        }
        else
        {
            int bestValue = 0;

            if (isMaximiser)//If it is the Maximisers Turn (Red)
            {
                bestValue = 0;

                for (int i = 0; i < node.children.Count; i++)
                {
                    bestValue = Mathf.Max(minimax(node.children[i], !isMaximiser, depth - 1, bestValue), bestValue);
                }
            }
            else//Else if it is the Minimisers Turn (Blue)
            {
                bestValue = 0;

                for (int i = 0; i < node.children.Count; i++)
                {
                    bestValue = Mathf.Min(minimax(node.children[i], !isMaximiser, depth - 1, bestValue), bestValue);
                }
            }

            return bestValue;
        }
    }

    public int UtilityFunction(Node node)
    {
        return -(node.simulation.GetScores().blueScore) + (node.simulation.GetScores().redScore);
    }
}
