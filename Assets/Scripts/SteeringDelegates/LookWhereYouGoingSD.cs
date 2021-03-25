using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookWhereYouGoingSD : SteeringBehaviour
{
    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        st.angular = SimulationManager.TurnAmountInDirection(personaje.orientacion,SimulationManager.VectorToDirection(personaje.velocidad));
        if (st.angular == 0) _finished = true;
        return st;
    }
}
