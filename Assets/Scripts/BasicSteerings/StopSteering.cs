using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSteering : SteeringBehaviour
{
    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        personaje.velocidad = Vector3.zero;
        _finishedLinear = true;
        return new Steering();
    }
}
