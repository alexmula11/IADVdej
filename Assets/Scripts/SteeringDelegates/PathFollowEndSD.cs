using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowEndSD : SteeringBehaviour
{

    protected List<Vector3> path;
    protected int currentPoint = 0;
    protected bool setup = false;
    protected PursueSD pursueSD = new PursueSD();
    protected Vector3 pathEnd;

    public PathFollowEndSD(List<Vector3> path)
    {
        this.path = path;
        if (path.Count>0) pathEnd = path[path.Count - 1];
    }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        if (currentPoint >= path.Count)
        {
            _finishedAngular = _finishedLinear = true;
            Vector3 cuadrao = SimManagerFinal.gridToPosition(SimManagerFinal.positionToGrid(personaje.posicion));
            personaje.fakeAvoid.posicion = cuadrao;
            personaje.fakeAvoid.moveTo(cuadrao);
            personaje.fakeMovement.posicion = cuadrao;
            personaje.fakeMovement.moveTo(cuadrao);
            return new Steering();
        }
        personaje.fakeAvoid.posicion = pathEnd;
        personaje.fakeAvoid.moveTo(pathEnd);
        personaje.fakeMovement.posicion = path[currentPoint];
        personaje.fakeMovement.moveTo(path[currentPoint]);

        pursueSD.target = personaje.fakeMovement;
        Steering movActual = pursueSD.getSteering(personaje);
        if (pursueSD.finishedLinear)
        {
            currentPoint++;
        }
        return movActual;
    }
}
