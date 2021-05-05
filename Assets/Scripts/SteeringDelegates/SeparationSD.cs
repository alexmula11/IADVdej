using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeparationSD : SteeringBehaviour
{
    private FleeSteering flee = new FleeSteering();
    private FaceSD face = new FaceSD();

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        personaje.group = SimulationManager.PersonajesCerca(personaje);
        foreach ( PersonajeBase person in personaje.group)
        {
            flee.target = person;
            st.linear += flee.getSteering(personaje).linear;
        }
        personaje.fakeMovement.posicion = personaje.posicion + st.linear;
        personaje.fakeMovement.transform.position = personaje.posicion + st.linear;
        face.target = personaje.fakeMovement;
        st.angular = face.getSteering(personaje).angular;
        _finishedLinear = _finishedAngular = personaje.group.Count == 0;
        return st;
    }
}
