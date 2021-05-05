using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArriveSteering : SteeringBehaviour
{
    protected new bool finishedAngular { get { return true; } }
    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        Vector3 distance = _target.posicion - personaje.posicion;
        if (distance.magnitude > _target.outterDetector)
        {
            //st.linear = (distance + _target.velocidad * Time.fixedDeltaTime).normalized * personaje.movAcc;
            //                        AAA     Un poco de predicsion   AAA
            st.linear = (distance.normalized - personaje.velocidad.normalized + _target.velocidad.normalized * Time.fixedDeltaTime).normalized * personaje.movAcc;
        }
        else if (distance.magnitude > _target.innerDetector)
        {
            st.linear = (distance.normalized - personaje.velocidad.normalized + _target.velocidad.normalized * Time.fixedDeltaTime).normalized * personaje.movAcc * distance.magnitude / _target.outterDetector;
        }
        else
        {
            st.linear = -personaje.velocidad.normalized * System.Math.Min(personaje.velocidad.magnitude, personaje.movAcc);
            _finishedLinear = true;
        }
        return st;
    }
}
