using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRTAChevychevSD : LRTASD
{
    public LRTAChevychevSD(bool[][] walls, Vector2 origin, Vector2 destiny) : base(walls, origin, destiny)
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
                    pesos[i][j] = (int)(System.Math.Max((System.Math.Abs(destiny.x - i)), (System.Math.Abs(destiny.y - j))));
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
