using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowingNOPathOffsetGridSD : SteeringBehaviour
{
    protected List<Vector2> path;
    protected int currentPoint = 0;
    protected bool setup = false;
    protected AStarSD aStar;

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
            float minDist = Mathf.Infinity;
            int nearestPoint = -1;
            for (int i = 0; i < path.Count; i++)
            {
                Vector2 personPos = SimManagerFinal.positionToGrid(personaje.posicion);
                if ((path[i] - personPos).magnitude < minDist)
                {
                    nearestPoint = i;
                    minDist = (path[i] - personPos).magnitude;
                }
            }
            setup = true;
            currentPoint = nearestPoint;
        }
        if (aStar.finishedLinear)
        {
            currentPoint = (currentPoint + 1) % path.Count;
        }
        personaje.fakeMovement.posicion = path[currentPoint];
        personaje.fakeMovement.moveTo(path[currentPoint]);
        return aStar.getSteering(personaje);
    }
}
