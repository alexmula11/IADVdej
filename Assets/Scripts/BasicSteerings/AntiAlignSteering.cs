using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiAlignSteering : SteeringBehaviour
{

    internal override Steering getSteering(PersonajeBase personaje)
    {
        float timeToTarget = 0.1f;
        float rotation = 0;
        float rotationSize = 0;
        float targetRotation = 0;
        Steering st = new Steering();
        st.linear = Vector3.zero;
        

        rotation = SimulationManager.TurnAmountInDirection(personaje.orientacion,SimulationManager.VectorToDirection(_target.posicion-personaje.posicion) + Mathf.PI);

        rotationSize = Mathf.Abs(rotation);

        if(rotationSize < target.innerAngleVision)
        {
            st.angular = 0;
            return st;
        }

        if(rotationSize > personaje.outterAngleVision)
        {
            targetRotation = personaje.rotSpeed; //maxima velocidad?
        }
        else
        {
            targetRotation = personaje.rotSpeed * rotationSize / personaje.outterAngleVision;
        }

        targetRotation *= (rotation/rotationSize);

        st.angular = (target.rotacion - personaje.rotacion) / timeToTarget;

        float angularAcelleration = Mathf.Abs(st.angular);

        if(angularAcelleration > personaje.maxAngAcc)
        {
            st.angular /= angularAcelleration;
            st.angular *= personaje.maxAngAcc;
        }
        return st;
    }
}
