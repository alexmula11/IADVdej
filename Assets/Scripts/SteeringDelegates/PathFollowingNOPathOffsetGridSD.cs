using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowingNOPathOffsetGridSD : SteeringBehaviour
{
    protected List<Vector2> path = new List<Vector2>();
    protected int currentPoint = 0;
    protected bool setup = false;
    protected List<Vector3> ruta = new List<Vector3>();
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
            currentPoint = nearestPoint;
        }
        if (pursueSD.finishedLinear)
        {
            currentPoint = (currentPoint + 1) % ruta.Count;
        }
        personaje.fakeMovement.innerDetector = personaje.innerDetector;
        personaje.fakeMovement.posicion = ruta[currentPoint];
        personaje.fakeMovement.moveTo(ruta[currentPoint]);
        pursueSD.target = personaje.fakeMovement;
        return pursueSD.getSteering(personaje);
    }
}
