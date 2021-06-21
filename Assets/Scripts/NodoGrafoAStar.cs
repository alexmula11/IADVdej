using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodoGrafoAStar
{
    protected internal Vector2 posicionGrid;
    protected internal float estimatedCost;//H
    protected internal float costFromOrigin;//G
    protected internal float totalCost;//F
    protected internal NodoGrafoAStar padre;

    //protected internal List<NodoGrafoAStar> adyacentes = new List<NodoGrafoAStar>();

    public NodoGrafoAStar(Vector2 pos, float _estimatedCost, float _costFromOrigin, NodoGrafoAStar _padre)
    {
        posicionGrid = pos;
        estimatedCost = _estimatedCost;
        costFromOrigin = _costFromOrigin;
        totalCost = estimatedCost + costFromOrigin;
        padre = _padre;
    }

}
