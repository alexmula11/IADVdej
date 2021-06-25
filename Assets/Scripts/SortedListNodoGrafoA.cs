using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortedListNodoGrafoA
{
    protected ArrayList lista = new ArrayList();

    protected internal void add(NodoGrafoAStar nodo)
    {
        int index = 0;
        foreach (NodoGrafoAStar nodito in lista)
        {
            if (nodito.totalCost > nodo.totalCost)
            {
                break;
            }
            index++;
        }
        if(index >= 0 && index < lista.Count)
        {
            lista.Insert(index, nodo);
        }
        else
        {
            lista.Add(nodo);
        }
    }

    protected internal void addOrReplace(NodoGrafoAStar nuevoNodo)
    {
        NodoGrafoAStar posibleaASustituir = null;
        bool estaEnListaOpen = false;
        int index = 0;
        foreach (NodoGrafoAStar noditoOpen in lista)
        {
            if (nuevoNodo.posicionGrid == noditoOpen.posicionGrid)
            {
                estaEnListaOpen = true;
                if (noditoOpen.totalCost > nuevoNodo.totalCost)
                {
                    posibleaASustituir = noditoOpen;
                }
                break;
            }
            index++;
        }
        if (posibleaASustituir != null)
        {
            lista.RemoveAt(index);
            lista.Insert(index,nuevoNodo);
            
        }
        else if (!estaEnListaOpen)
        {
            lista.Add(nuevoNodo);
        }
    }

    internal NodoGrafoAStar pop()
    {
        NodoGrafoAStar ret = (NodoGrafoAStar)lista[0];
        lista.RemoveAt(0);
        return ret;
    }

}
