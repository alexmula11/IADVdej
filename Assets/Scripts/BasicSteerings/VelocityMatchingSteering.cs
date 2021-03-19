using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMatchingSteering : SteeringBehaviour
{

    internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        float timeToTarget = 0.1f;

        st.linear = _target.velocidad - personaje.velocidad;
        st.linear /= timeToTarget;

        if(st.linear.magnitude > personaje.movAcc)
        {
            st.linear = st.linear.normalized * personaje.movAcc;
        }
        st.angular = 0;
        return st;
    }

}
