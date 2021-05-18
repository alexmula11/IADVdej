using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRTASD : SteeringBehaviour
{
    private int[][] pesos;

    private PursueSD pursueSD = new PursueSD();

    private bool setup = true;

    public LRTASD(bool[][] walls, Vector2 origin, Vector2 destiny)
    {
        pesos = new int[walls.Length][];
        for (int i=0; i < pesos.Length; i++)
        {
            pesos[i] = new int[walls[i].Length];
            for (int j=0; j < pesos[i].Length; j++)
            {
                if (walls[i][j])
                {
                    pesos[i][j] = int.MaxValue;
                }
                else 
                    pesos[i][j] = (int)((System.Math.Abs(destiny.x - origin.x)) + (System.Math.Abs(destiny.y - origin.y)));
            }
        }
    }

    private int[] minimalSpace(Vector2 pos)
    {
        int[] costs = new int[4];
        costs[0] = pesos[(int)pos.x][(int)pos.y + 1];
        costs[1] = pesos[(int)pos.x+1][(int)pos.y];
        costs[2] = pesos[(int)pos.x][(int)pos.y - 1];
        costs[3] = pesos[(int)pos.x-1][(int)pos.y];
        return costs;
    }

    private Vector2 nextMove(int[] minimalSpace)
    {
        int minorCostIndex = -1, minorCost = int.MaxValue;
        for (int i=0; i < minimalSpace.Length; i++)
        {
            if (minimalSpace[i] < minorCost)
            {
                minorCost = minimalSpace[i];
                minorCostIndex = 1;
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
        Vector2 personajePos = new Vector2((int)personaje.posicion.x / SimManagerLRTA.BLOCKSIZE + pesos.Length/2 , (int)personaje.posicion.z / SimManagerLRTA.BLOCKSIZE + pesos[0].Length/2);
        if (pesos[(int)personajePos.x][(int)personajePos.y] != 0)
        {
            if (setup || pursueSD.finishedLinear)
            {
                Vector2 nextPosition = nextMove(minimalSpace(personajePos));
                Vector2 proxMov = personajePos + nextPosition;

                pesos[(int)personajePos.x][(int)personajePos.y] = pesos[(int)proxMov.x][(int)proxMov.y] + 1;

                personaje.fakeMovement.posicion = new Vector3((proxMov.x - pesos.Length / 2) * SimManagerLRTA.BLOCKSIZE, 0, (proxMov.y - pesos[0].Length / 2) * SimManagerLRTA.BLOCKSIZE);
                personaje.fakeMovement.transform.position = new Vector3((proxMov.x - pesos.Length / 2) * SimManagerLRTA.BLOCKSIZE, 0, (proxMov.y - pesos[0].Length / 2) * SimManagerLRTA.BLOCKSIZE);
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
