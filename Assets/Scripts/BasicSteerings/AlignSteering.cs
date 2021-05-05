using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignSteering : SteeringBehaviour
{
    protected new bool finishedLinear { get { return true; } }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        _finishedAngular = false;
        //float timeToTarget = 0.1f;
        float rotation = 0;
        float rotationSize = 0;
        float targetRotation = 0;
        Steering st = new Steering();
        st.linear = Vector3.zero;

        //rotation = SimulationManager.TurnAmountInDirection(personaje.orientacion,SimulationManager.VectorToDirection(_target.posicion-personaje.posicion));
        rotation = SimulationManager.TurnAmountInDirection(personaje.orientacion,_target.orientacion);
        rotationSize = Mathf.Abs(rotation);

        if(rotationSize < target.innerAngleVision)
        {
            st.angular = 0;
            _finishedAngular = true;
            return st;
        }
        _finishedAngular = false;

        if(rotationSize > personaje.outterAngleVision)
        {
            targetRotation = System.Math.Sign(rotation) * personaje.rotSpeed; //maxima velocidad?
        }
        else
        {
            targetRotation = System.Math.Sign(rotation) * personaje.rotSpeed * rotationSize / personaje.outterAngleVision;
        }

        st.angular = targetRotation;

        float angularAcelleration = Mathf.Abs(st.angular);

        if(angularAcelleration > personaje.rotSpeed)
        {
            st.angular = System.Math.Sign(st.angular) * personaje.rotSpeed;
        }

        
        return st;
    }

}
