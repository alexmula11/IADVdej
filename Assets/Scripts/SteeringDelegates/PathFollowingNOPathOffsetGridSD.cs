using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowingNOPathOffsetGridSD : SteeringBehaviour
{
    protected List<Vector2> path = new List<Vector2>();
    protected int currentPoint = 0;
    protected int currentStartingPoint = 0;
    protected bool setup = false;
    protected bool startingsetup = false;
    protected List<Vector3> ruta = new List<Vector3>();
    protected List<Vector3> startingruta = new List<Vector3>();
    protected PursueSD pursueSD = new PursueSD();

    public PathFollowingNOPathOffsetGridSD(List<Vector3> path, StatsInfo.TIPO_TERRENO[][] terrenos)
    {
        foreach (Vector3 pos in path)
        {
            this.path.Add(SimManagerFinal.positionToGrid(pos));
        }
    }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        if (!setup)
        {
            Vector3 puntoActual = path[0];
            for (int i = 1; i < path.Count; i++)
            {
                List<Vector3> recorridoActual = SimManagerFinal.aStarPathV3(puntoActual, path[i], personaje.tipo, personaje is PersonajePlayer);
                ruta.AddRange(recorridoActual);
                puntoActual = path[i];
            }
            List<Vector3> recorridoFinal = SimManagerFinal.aStarPathV3(path[path.Count-1], path[0], personaje.tipo, personaje is PersonajePlayer);
            ruta.AddRange(recorridoFinal);

            float minDist = Mathf.Infinity;
            int nearestPoint = -1;
            for (int i = 0; i < ruta.Count; i++)
            {
                if ((ruta[i] - personaje.posicion).magnitude < minDist)
                {
                    nearestPoint = i;
                    minDist = (ruta[i] - personaje.posicion).magnitude;
                }
            }
            setup = true;
            currentStartingPoint = nearestPoint;
            startingruta = SimManagerFinal.aStarPathV3(SimManagerFinal.positionToGrid(personaje.posicion),SimManagerFinal.positionToGrid(ruta[nearestPoint]), personaje.tipo, personaje is PersonajePlayer);
        }

        if(!startingsetup)
        {
            if (startingruta.Count > 0)
            {
                //preguntar a alex si se acuerda
                personaje.fakeMovement.innerDetector = personaje.innerDetector;
                personaje.fakeMovement.posicion = startingruta[currentPoint];
                personaje.fakeMovement.moveTo(startingruta[currentPoint]);
                personaje.fakeAvoid.posicion = startingruta[startingruta.Count - 1];
                personaje.fakeAvoid.moveTo(startingruta[startingruta.Count - 1]);
                if (pursueSD.finishedLinear)
                {
                    currentPoint = (currentPoint + 1);
                    if (currentPoint >= startingruta.Count)
                    {
                        startingsetup = true;
                        currentPoint = currentStartingPoint;
                        for (int i=currentPoint; i < ruta.Count; i++)
                        {
                            bool found = false;
                            for (int j = 0; j < path.Count; j++)
                            {
                                if (ruta[i] == SimManagerFinal.gridToPosition(path[j]))
                                {
                                    personaje.fakeAvoid.posicion = ruta[i];
                                    personaje.fakeAvoid.moveTo(ruta[i]);
                                    found = true;
                                    break;
                                }
                            }
                            if (found) break;
                        }
                    }
                }
            }
            else
            {
                startingsetup = true;
                currentPoint = currentStartingPoint;
                for (int i = currentPoint; i < ruta.Count; i++)
                {
                    bool found = false;
                    for (int j = 0; j < path.Count; j++)
                    {
                        if (ruta[i] == SimManagerFinal.gridToPosition(path[j]))
                        {
                            personaje.fakeAvoid.posicion = ruta[i];
                            personaje.fakeAvoid.moveTo(ruta[i]);
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
            }
        }
        else
        {
            if (pursueSD.finishedLinear)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    if (path[i] == SimManagerFinal.positionToGrid(ruta[currentPoint]))
                    {
                        int next = (i + 1) % path.Count;
                        Vector3 nextRoutePoint = SimManagerFinal.gridToPosition(path[next]);
                        personaje.fakeAvoid.posicion = nextRoutePoint;
                        personaje.fakeAvoid.moveTo(nextRoutePoint);
                    }
                }
                currentPoint = (currentPoint + 1) % ruta.Count;
            }
            personaje.fakeMovement.innerDetector = personaje.innerDetector;
            personaje.fakeMovement.posicion = ruta[currentPoint];
            personaje.fakeMovement.moveTo(ruta[currentPoint]);
        }
        pursueSD.target = personaje.fakeMovement;
        return pursueSD.getSteering(personaje);
    }
}
