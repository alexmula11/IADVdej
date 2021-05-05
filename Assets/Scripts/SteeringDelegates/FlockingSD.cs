using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingSD : SteeringBehaviour
{
    private const float PURSUE_LEADER_PERCENT = 1f;

    private GroupAlignSD grAlSD = new GroupAlignSD();
    private CohesionSD chSD = new CohesionSD();
    private SeparationSD sepSD = new SeparationSD();
    private PursueSD pursueSD = new PursueSD();
    private LookWhereYouGoingSD lookSD = new LookWhereYouGoingSD();

    private const float distanceDivision = 20f;

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        personaje.group = SimulationManager.PersonajesCerca(personaje);
        int lejanos = 0, cercanos = 0;
        foreach (PersonajeBase person in personaje.group)
        {
            if ((person.posicion - personaje.posicion).magnitude < 10)
            {
                cercanos++;
            }
            else
            {
                lejanos++;
            }
        }
        Steering st = new Steering();
        pursueSD.target = _target;
        st.linear = chSD.getSteering(personaje).linear * (lejanos / personaje.group.Count)
            + sepSD.getSteering(personaje).linear * cercanos / personaje.group.Count
            + pursueSD.getSteering(personaje).linear * PURSUE_LEADER_PERCENT;
        personaje.fakeMovement.posicion = personaje.posicion + st.linear;
        personaje.fakeMovement.moveTo(personaje.posicion + st.linear);
        if (st.linear == Vector3.zero)
        {
            st.angular = grAlSD.getSteering(personaje).angular;
        }
        else
        {
            st.angular = lookSD.getSteering(personaje).angular;
        }
        personaje.fakeAlign.orientacion = st.angular;
        personaje.fakeAlign.transform.eulerAngles = new Vector3(0, st.angular * Bodi.RadianesAGrados, 0);
        return st;
    }
}
