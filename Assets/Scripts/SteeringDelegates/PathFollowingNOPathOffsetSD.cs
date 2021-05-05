using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowingNOPathOffsetSD : SteeringBehaviour
{
    protected List<Vector3> path;
    protected int currentPoint = 0;
    protected bool setup = false;
    protected PursueSD pursueSD = new PursueSD();

    public PathFollowingNOPathOffsetSD(List<Vector3> path)
    {
        this.path = path;
    } 

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        if (!setup)
        {
            float minDist = Mathf.Infinity;
            int nearestPoint = -1;
            for (int i = 0; i < path.Count; i++)
            {
                if ((path[i] -personaje.posicion).magnitude < minDist)
                {
                    nearestPoint = i;
                    minDist = (path[i] - personaje.posicion).magnitude;
                }
            }
            setup = true;
            currentPoint = nearestPoint;
        }
        if (pursueSD.finishedLinear)
        {
            currentPoint = (currentPoint + 1) % path.Count;
        }
        pursueSD.target = personaje.fakeMovement;
        personaje.fakeMovement.posicion = path[currentPoint];
        personaje.fakeMovement.moveTo(path[currentPoint]);
        return pursueSD.getSteering(personaje);
    }
}
