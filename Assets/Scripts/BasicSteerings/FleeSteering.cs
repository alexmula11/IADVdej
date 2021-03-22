using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeSteering : SteeringBehaviour
{
    internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        /*if ((_target.posicion - personaje.posicion).magnitude > _target.innerDetector)
        {
            st.linear = (personaje.posicion - _target.posicion).normalized * personaje.movAcc;
        }*/
        st.linear = (personaje.posicion - _target.posicion).normalized * personaje.movAcc;
        return st;
    }
}
