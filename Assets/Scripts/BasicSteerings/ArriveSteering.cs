using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArriveSteering : SteeringBehaviour
{
    internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        Vector3 distance = _target.posicion - personaje.posicion;
        if (distance.magnitude > _target.outterDetector)
        {
            st.linear = (distance + _target.velocidad * Time.fixedDeltaTime).normalized * personaje.movAcc;
            //                        AAA     Un poco de predicsion   AAA
        }
        else if (distance.magnitude > _target.innerDetector)
        {
            st.linear = (distance + _target.velocidad * Time.fixedDeltaTime).normalized * personaje.movAcc * distance.magnitude / _target.outterDetector;
        }
        return st;
    }
}
