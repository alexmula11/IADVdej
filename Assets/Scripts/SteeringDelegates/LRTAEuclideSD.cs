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

    protected override List<NodoGrafo> generateMinimalSpace(NodoGrafo ng)
    {
        List<NodoGrafo> listanodos = new List<NodoGrafo>();

        //cruz
        listanodos.Add(new NodoGrafo(new Vector2(ng.posicionGrid.x, ng.posicionGrid.y+1), pesos[(int)ng.posicionGrid.x][(int)ng.posicionGrid.y + 1]));
        listanodos.Add(new NodoGrafo(new Vector2(ng.posicionGrid.x+1, ng.posicionGrid.y), pesos[(int)ng.posicionGrid.x+1][(int)ng.posicionGrid.y]));
        listanodos.Add(new NodoGrafo(new Vector2(ng.posicionGrid.x, ng.posicionGrid.y-1), pesos[(int)ng.posicionGrid.x][(int)ng.posicionGrid.y - 1]));
        listanodos.Add(new NodoGrafo(new Vector2(ng.posicionGrid.x-1, ng.posicionGrid.y), pesos[(int)ng.posicionGrid.x-1][(int)ng.posicionGrid.y]));

        //diagonales
        listanodos.Add(new NodoGrafo(new Vector2(ng.posicionGrid.x+1, ng.posicionGrid.y+1), pesos[(int)ng.posicionGrid.x+1][(int)ng.posicionGrid.y + 1]));
        listanodos.Add(new NodoGrafo(new Vector2(ng.posicionGrid.x-1, ng.posicionGrid.y-1), pesos[(int)ng.posicionGrid.x-1][(int)ng.posicionGrid.y - 1]));
        listanodos.Add(new NodoGrafo(new Vector2(ng.posicionGrid.x-1, ng.posicionGrid.y+1), pesos[(int)ng.posicionGrid.x-1][(int)ng.posicionGrid.y + 1]));
        listanodos.Add(new NodoGrafo(new Vector2(ng.posicionGrid.x+1, ng.posicionGrid.y-1), pesos[(int)ng.posicionGrid.x+1][(int)ng.posicionGrid.y - 1]));

        return listanodos;
    }

}
