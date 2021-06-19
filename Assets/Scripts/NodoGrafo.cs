using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodoGrafo 
{
    protected internal Vector2 posicionGrid;
    protected internal int peso;

    protected internal List<NodoGrafo> adyacentes = new List<NodoGrafo>();

    public NodoGrafo(Vector2 pos, int _peso)
    {
        posicionGrid = pos;
        peso = _peso;
    }

    public void addAdyacentes(List<NodoGrafo> noditos){
        foreach(NodoGrafo nd in noditos)
            adyacentes.Add(nd);
    }

}
