using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LRTAEuclideSD : LRTASD
{
    public LRTAEuclideSD(bool[][] walls, Vector2 origin, Vector2 destiny) : base(walls, origin, destiny)
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
                    pesos[i][j] = (int)System.Math.Round((destiny-new Vector2(i,j)).magnitude);
                }
            }
        }
    }
}
