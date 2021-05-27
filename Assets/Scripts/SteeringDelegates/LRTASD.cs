using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class LRTASD : SteeringBehaviour
{
    protected int[][] pesos;

    protected PursueSD pursueSD = new PursueSD();

    protected bool setup = true;

    protected Vector2 destiny;

    public LRTASD(bool[][] walls, Vector2 origin, Vector2 destiny)
    {
        this.destiny = destiny;
        populateWeights(walls,origin, destiny);
    }

    protected abstract void populateWeights(bool[][] walls, Vector2 origin, Vector2 destiny);

    private int[] minimalSpace(Vector2 pos)
    {
        int[] costs = new int[4];
        costs[0] = pesos[(int)pos.x][(int)pos.y + 1];
        costs[1] = pesos[(int)pos.x+1][(int)pos.y];
        costs[2] = pesos[(int)pos.x][(int)pos.y - 1];
        costs[3] = pesos[(int)pos.x-1][(int)pos.y];
        return costs;
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
    private Vector2 nextMove(int[] minimalSpace, Vector2 position)
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
    }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        Vector2 personajePos = SimManagerLRTA.positionToGrid(personaje.posicion);
        if (pesos[(int)personajePos.x][(int)personajePos.y] != 0)
        {
            if (setup || pursueSD.finishedLinear)
            {
                Vector2 nextPosition = nextMove(minimalSpace(personajePos),SimManagerLRTA.positionToGrid(personaje.posicion));
                Vector2 proxMov = personajePos + nextPosition;

                pesos[(int)personajePos.x][(int)personajePos.y] = pesos[(int)proxMov.x][(int)proxMov.y] + 1;

                int altura = (int)(proxMov.y - pesos[0].Length / 2);

                personaje.fakeMovement.posicion = SimManagerLRTA.gridToPosition(proxMov);
                personaje.fakeMovement.transform.position = SimManagerLRTA.gridToPosition(proxMov);
                personaje.fakeMovement.innerDetector = personaje.innerDetector;

                pursueSD.target = personaje.fakeMovement;
                setup = false;
            }
            return pursueSD.getSteering(personaje);
        }
        else
        {
            _finishedLinear = true;
            return new Steering();
        }
    }
}
