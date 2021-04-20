using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekAcceleratedSteering : SteeringBehaviour
{

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        float distance = (_target.posicion - personaje.posicion).magnitude;
        if (distance > _target.innerDetector)
        {
            //2nda manera, acumulación de velocidad
            //st.linear = ((_target.posicion - personaje.posicion) + _target.velocidad * Time.fixedDeltaTime).normalized * personaje.movAcc;
            //                                                      AAA     Un poco de predicsion   AAA
            //1ra manera, superposición de velocidad (velocidad deseada en el primer frame) velocidad desada = velocidad que tienes + velocidad que pasa el steering 
            // --> velocidad que pasa el steering = velocidad deseada - velocidad que tienes
            Vector3 desiredSpeed = (_target.posicion - personaje.posicion).normalized * System.Math.Min(personaje.maxMovSpeed, distance);
            st.linear = (desiredSpeed - personaje.velocidad + _target.velocidad * Time.fixedDeltaTime).normalized * System.Math.Min(personaje.movAcc, distance);
            //                                                      AAA     Un poco de predicsion   AAA
            _finished = false;
        }
        else
        {
            _finished = true;
        }
        return st;
    }

    

}
