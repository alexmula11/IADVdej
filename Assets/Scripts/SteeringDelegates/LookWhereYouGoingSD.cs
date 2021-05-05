using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookWhereYouGoingSD : SteeringBehaviour
{
    protected new bool finishedLinear { get { return true; } }
    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        st.angular = SimulationManager.TurnAmountInDirection(personaje.orientacion,SimulationManager.VectorToDirection(personaje.velocidad));
        _finishedAngular = st.angular == 0;
        return st;
    }
}
