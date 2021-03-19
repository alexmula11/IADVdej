using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignSteering : SteeringBehaviour
{
    

    internal override Steering getSteering(PersonajeBase personaje)
    {
        float timeToTarget = 0.1f;
        float rotation = 0;
        float rotationSize = 0;
        float targetRotation = 0;
        Steering st = new Steering();
        st.linear = Vector3.zero;
        

        rotation = SimulationManager.TurnAmountInDirection(personaje.orientacion,SimulationManager.VectorToDirection(_target.posicion-personaje.posicion));

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



        /*if (st.angular < 0.5f)
        {
            st.angular = 0f;
        }
        /*Vector3 distance = _target.posicion - personaje.posicion;
        float faceDirection = SimulationManager.VectorToDirection(new Vector2(distance.x,distance.z));
        float turnSide = SimulationManager.TurnAmountInDirection(personaje.orientacion, faceDirection);
        st.angular = turnSide;
        float turnAmount = personaje.rotSpeed * Time.fixedDeltaTime;
        if (System.Math.Abs(turnSide) <= turnAmount)
        {
            st.angular = faceDirection;
        }
        else
        {
            float newAngle = personaje.orientacion + turnAmount * System.Math.Sign(turnSide);
            if (newAngle > 180) newAngle = -(180 - (newAngle - 180));
            else if (newAngle < -180) newAngle = 180 + (newAngle + 180);
            st.angular = newAngle;
            //transform.eulerAngles = new Vector3(0, newAngle, 0); //ASK IF CAN TO PROF
        }*/
        return st;
    }

}
