using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class LRTASD : SteeringBehaviour
{

    protected GrafoBusqueda grafoB;
    protected NodoGrafo nodoActual;

    protected int[][] pesos;

    protected PursueSD pursueSD = new PursueSD();

    protected bool setup = true;

    protected Vector2 destiny;

    protected int MAX_ROUTE = 15;

    public LRTASD(bool[][] walls, Vector2 origin, Vector2 destiny)
    {
        this.destiny = destiny;
        populateWeights(walls,origin, destiny);
        nodoActual = new NodoGrafo(origin,pesos[(int)origin.x][(int)origin.y]);
        grafoB = new GrafoBusqueda(nodoActual);
    }


    //Generador de pesos segun la metrica que se va a usar
    protected abstract void populateWeights(bool[][] walls, Vector2 origin, Vector2 destiny);

    protected abstract List<NodoGrafo> generateMinimalSpace(NodoGrafo nd);

    private void minimalSpace(Vector2 pos)
    {
        nodoActual.addAdyacentes(generateMinimalSpace(nodoActual));
        nodoActual.adyacentes = nodoActual.adyacentes.FindAll(e => e.peso != int.MaxValue);
    }

    /*
    private Vector2 nextMove(int[] minimalSpace, Vector2 position)
    {

        int minorCostIndex = -1, minorCost = int.MaxValue;
        float minorDistance = float.MaxValue;
        for (int i=0; i < minimalSpace.Length; i++)
        {
            if (minimalSpace[i]!=int.MaxValue && minimalSpace[i] <= minorCost)
            {
                float distance=0;
                switch (i)
                {
                    case 0: distance = (destiny - (position+ new Vector2(0, 1))).magnitude;
                        break;
                    case 1:
                        distance = (destiny - (position + new Vector2(1, 0))).magnitude;
                        break;
                    case 2:
                        distance = (destiny - (position + new Vector2(0, -1))).magnitude;
                        break;
                    case 3:
                        distance = (destiny - (position + new Vector2(-1, 0))).magnitude;
                        break;
                    default:
                        break;
                }
                if (minimalSpace[i] < minorCost)
                {
                    minorCost = minimalSpace[i];
                    minorCostIndex = i;
                    minorDistance = distance;
                }else if (distance<minorDistance)
                {
                    minorCost = minimalSpace[i];
                    minorCostIndex = i;
                    minorDistance = distance;
                }
            }
        }

        switch (minorCostIndex)
        {
            case 0: return new Vector2(0, 1);
            case 1: return new Vector2(1, 0);
            case 2: return new Vector2(0, -1);
            case 3: return new Vector2(-1, 0);
            default: return new Vector2(0, 0);
        }
    }
    */

    protected NodoGrafo nextMove()
    {
        int minorCost = int.MaxValue;
        NodoGrafo nextNode = null;

        foreach(NodoGrafo nodito in nodoActual.adyacentes)
        {
            if(nodito.peso < minorCost){
                nextNode = nodito;
                minorCost = nodito.peso;
            }
        }
        return nextNode;
    }

    /*private Vector2 nextMove(int[] minimalSpace, Vector2 position)
    {

        int minorCostIndex = -1, minorCost = int.MaxValue;
        for (int i = 0; i < minimalSpace.Length; i++)
        {
            if (minimalSpace[i] < minorCost)
            {
                minorCost = minimalSpace[i];
                minorCostIndex = i;
            }
        }
        switch (minorCostIndex)
        {
            case 0: return new Vector2(0, 1);
            case 1: return new Vector2(1, 0);
            case 2: return new Vector2(0, -1);
            case 3: return new Vector2(-1, 0);
            default: return new Vector2(0, 0);
        }
    }*/

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        Vector2 personajePos = SimManagerLRTA.positionToGrid(personaje.posicion);
        if (pesos[(int)Math.Round(personajePos.x)][(int)Math.Round(personajePos.y)] != 0)
        {
            if (setup || pursueSD.finishedLinear)
            {
                minimalSpace(personajePos);
                NodoGrafo nextNode = nextMove();

                pesos[(int)personajePos.x][(int)personajePos.y] = pesos[(int)nextNode.posicionGrid.x][(int)nextNode.posicionGrid.y] + 1;

                personaje.fakeMovement.posicion = SimManagerLRTA.gridToPosition(nextNode.posicionGrid);
                personaje.fakeMovement.transform.position = SimManagerLRTA.gridToPosition(nextNode.posicionGrid);
                //personaje.innerDetector = 0.5f;
                personaje.fakeMovement.innerDetector = personaje.innerDetector;

                pursueSD.target = personaje.fakeMovement;
                setup = false;

                nodoActual = nextNode;
            }
            return pursueSD.getSteering(personaje);
        }
        else
        {
            _finishedLinear = true;
            return new Steering();
        }
    }

    //calculamos la posible ruta futura
    private List<Vector2> calculateRoute()
    {
        for(int i = 0; i < MAX_ROUTE; i++){

        }
        return null;
    }
}
