using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeSteering : SteeringBehaviour
{
    protected new bool finishedAngular { get { return true; } }
    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        /*if ((_target.posicion - personaje.posicion).magnitude > _target.innerDetector)
        {
            st.linear = (personaje.posicion - _target.posicion).normalized * personaje.movAcc;
        }*/
        //NECESITAMOS UNA CONDICION DE CIERRE AQUI TAMBIEN
        st.linear = (personaje.posicion - _target.posicion).normalized /*- personaje.velocidad.normalized*Time.fixedDeltaTime).normalized*/ * personaje.movAcc;
        return st;
    }
}
