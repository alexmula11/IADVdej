using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekAcceleratedSteering : SteeringBehaviour
{

    internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        /*if ((_target.posicion-personaje.posicion).magnitude > _target.innerDetector)
        {
            st.linear = ((_target.posicion - personaje.posicion) + _target.velocidad * Time.fixedDeltaTime).normalized * personaje.movAcc;
            //                                                      AAA     Un poco de predicsion   AAA
        }*/
        st.linear = ((_target.posicion - personaje.posicion) + _target.velocidad * Time.fixedDeltaTime).normalized * personaje.movAcc;
        //                                                      AAA     Un poco de predicsion   AAA
        return st;
    }

    

}
