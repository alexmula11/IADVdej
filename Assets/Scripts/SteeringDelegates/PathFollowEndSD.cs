﻿using System.Collections;
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
        pathEnd = path[path.Count - 1];
    }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        personaje.fakeAvoid.posicion = pathEnd;
        personaje.fakeAvoid.moveTo(pathEnd);
        if (pursueSD.finishedLinear)
        {
            currentPoint++;
            if (currentPoint>= path.Count)
            {
                _finishedAngular = _finishedLinear = true;
                return new Steering();
            }
        }
        pursueSD.target = personaje.fakeMovement;
        personaje.fakeMovement.posicion = path[currentPoint];
        personaje.fakeMovement.moveTo(path[currentPoint]);
        return pursueSD.getSteering(personaje);
    }
}
