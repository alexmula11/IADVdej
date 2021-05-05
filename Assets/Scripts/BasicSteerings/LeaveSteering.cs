using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveSteering : SteeringBehaviour
{
    protected new bool finishedAngular { get { return true; } }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        _finishedLinear = false;
        Steering st = new Steering();
        Vector3 distance = personaje.posicion - _target.posicion;
        if (distance.magnitude < _target.innerDetector)
        {
            st.linear = (distance.normalized - personaje.velocidad.normalized + _target.velocidad.normalized * Time.fixedDeltaTime).normalized * personaje.movAcc;
        }
        else if (distance.magnitude < _target.outterDetector)
        {
            st.linear = (distance.normalized - personaje.velocidad.normalized + _target.velocidad.normalized * Time.fixedDeltaTime).normalized * personaje.movAcc *(1- distance.magnitude / _target.outterDetector);
            //                        AAA     Un poco de predicsion   AAA                                       AAA Mas rápido cuanto más cerca AAA
        }
        else
        {
            st.linear = -personaje.velocidad.normalized * System.Math.Min(personaje.velocidad.magnitude, personaje.movAcc);
            _finishedLinear = true;
        }
        return st;
    }
}
