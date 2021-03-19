using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekAcceleratedSteering : SteeringBehaviour
{

    internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        if ((_target.posicion-personaje.posicion).magnitude > _target.innerDetector)
        {
            st.linear = (_target.posicion - personaje.posicion).normalized*personaje.movAcc;
        }
        /*float movAmount = personaje.currentMovementSpeed * Time.fixedDeltaTime;
        if (toMove.magnitude - personaje.innerDetector <= movAmount)
        {
            st.linear = Vector3.zero;
        }
        else
        {
            //ACCELERATED MOVEMENT - PREGUNTAR LUIS DANIEL
            if (personaje.currentMovementSpeed <= personaje.maxMovSpeed - personaje.movAcc * Time.fixedDeltaTime)
            {
                st.linear = toMove.normalized * (personaje.currentMovementSpeed + personaje.movAcc * Time.fixedDeltaTime);
            }
            else
            {
                st.linear = toMove.normalized * personaje.maxMovSpeed;
            }
        }*/
        return st;
    }

    

}
