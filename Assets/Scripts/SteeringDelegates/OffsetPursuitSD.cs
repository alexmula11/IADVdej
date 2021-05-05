using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetPursuitSD : SteeringBehaviour
{
    protected internal Vector3 offset;
    protected internal PursueSD pursueSD = new PursueSD();

    public OffsetPursuitSD()
    {

    }

    public OffsetPursuitSD(Vector3 offset)
    {
        this.offset = offset;
    }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        personaje.fakeMovement.posicion = _target.posicion + offset;
        personaje.fakeMovement.innerDetector = personaje.innerDetector;
        personaje.fakeMovement.moveTo(_target.posicion + offset);
        pursueSD.target = personaje.fakeMovement;
        Steering st = pursueSD.getSteering(personaje);
        _finishedLinear = pursueSD.finishedLinear;
        _finishedAngular = pursueSD.finishedAngular;
        return st;
    }
}
