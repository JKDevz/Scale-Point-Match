using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimax
{
    public static int DoMinimax(Node node, bool isMaximiser, int depth, int score)
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
                    bestValue = Mathf.Max(DoMinimax(node.children[i], !isMaximiser, depth - 1, bestValue), bestValue);
                }
            }
            else//Else if it is the Minimisers Turn (Blue)
            {
                bestValue = 0;

                for (int i = 0; i < node.children.Count; i++)
                {
                    bestValue = Mathf.Min(DoMinimax(node.children[i], !isMaximiser, depth - 1, bestValue), bestValue);
                }
            }

            node.minimaxValue = bestValue;
            return bestValue;
        }
    }

    public static int UtilityFunction(Node node)
    {
        node.minimaxValue = -(node.simulation.GetScores().blueScore) + (node.simulation.GetScores().redScore);
        return node.minimaxValue;
    }
}
