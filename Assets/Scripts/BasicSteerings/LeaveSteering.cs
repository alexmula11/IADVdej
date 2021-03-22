using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveSteering : SteeringBehaviour
{
    internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        Vector3 distance = personaje.posicion - _target.posicion;
        if (distance.magnitude < _target.innerDetector)
        {
            st.linear = (distance + _target.velocidad * Time.fixedDeltaTime).normalized * personaje.movAcc;
        }
        else if (distance.magnitude < _target.outterDetector)
        {
            st.linear = (distance + _target.velocidad * Time.fixedDeltaTime).normalized * personaje.movAcc *(1- distance.magnitude / _target.outterDetector);
            //                        AAA     Un poco de predicsion   AAA                                       AAA Mas rápido cuanto más cerca AAA
        }
        return st;
    }
}
