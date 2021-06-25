using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodoGrafoAStar : IHeapItem<NodoGrafoAStar>
{
    protected internal Vector2 posicionGrid;
    protected internal float estimatedCost;//H
    protected internal float costFromOrigin;//G
    protected internal float totalCost;//F
    protected internal NodoGrafoAStar padre;
    public int heapIndex;

    //protected internal List<NodoGrafoAStar> adyacentes = new List<NodoGrafoAStar>();

    public NodoGrafoAStar(Vector2 pos, float _estimatedCost, float _costFromOrigin, NodoGrafoAStar _padre)
    {
        posicionGrid = pos;
        estimatedCost = _estimatedCost;
        costFromOrigin = _costFromOrigin;
        totalCost = estimatedCost + costFromOrigin;
        padre = _padre;
    }

    public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

    public int CompareTo(NodoGrafoAStar nodeToCompare) {
		int compare = totalCost.CompareTo(nodeToCompare.totalCost);
		if (compare == 0) {
			compare = estimatedCost.CompareTo(nodeToCompare.estimatedCost);
		}
		return -compare;
	}

    public override bool Equals(object obj)
    {
        Debug.Log("compruebo");
        if (obj.GetType() != typeof (NodoGrafoAStar)) return false;
        return ((NodoGrafoAStar)obj).posicionGrid == this.posicionGrid;
    }

}
