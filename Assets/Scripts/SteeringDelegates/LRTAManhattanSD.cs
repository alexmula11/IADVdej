using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRTAManhattanSD : LRTASD
{
    public LRTAManhattanSD(bool[][] walls, Vector2 origin, Vector2 destiny) : base(walls, origin, destiny)
    {
    }

    protected override void populateWeights(bool[][] walls, Vector2 origin, Vector2 destiny)
    {
        pesos = new int[walls.Length][];
        for (int i = 0; i < pesos.Length; i++)
        {
            pesos[i] = new int[walls[i].Length];
            for (int j = 0; j < pesos[i].Length; j++)
            {

                if (walls[i][j])
                {
                    pesos[i][j] = int.MaxValue;
                }
                else
                {
                    pesos[i][j] = (int)((System.Math.Abs(destiny.x - i)) + (System.Math.Abs(destiny.y - j)));
                }
            }
        }
    }
}
