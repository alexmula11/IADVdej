using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingSD : SteeringBehaviour
{

    private GroupAlignSD grAlSD = new GroupAlignSD();
    private CohesionSD chSD = new CohesionSD();
    private SeparationSD sepSD = new SeparationSD();
    private PursueSD pursueSD = new PursueSD();
    private LookWhereYouGoingSD lookSD = new LookWhereYouGoingSD();

    private const float distanceDivision = 20f;

    internal float chPercentDyn=0, sepPercentDyn =0, followLeaderPercentDyn = 0;
    private float chPercent=1, sepPercent=1, pLeaderPercent = 1;

    public FlockingSD(float chPercent, float sepPercent, float pLeaderPercent)
    {
        this.chPercent = chPercent;
        this.sepPercent = sepPercent;
        this.pLeaderPercent = pLeaderPercent;
    }

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
        chPercentDyn = ((float)lejanos / personaje.group.Count) * chPercent;
        sepPercentDyn = ((float)cercanos / personaje.group.Count) * sepPercent;
        followLeaderPercentDyn = pLeaderPercent * System.Math.Min(2,(_target.posicion - personaje.posicion).magnitude/10);

        st.linear = chSD.getSteering(personaje).linear * chPercentDyn
            + sepSD.getSteering(personaje).linear * sepPercentDyn
            + pursueSD.getSteering(personaje).linear * followLeaderPercentDyn;
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
