using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueSD : SteeringBehaviour
{
    private FaceSD face = new FaceSD();
    private SeekAcceleratedSteering skAccSt = new SeekAcceleratedSteering();

    public PursueSD(PersonajeBase target)
    {
        skAccSt.target = face.target = _target = target;
    }

    internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        st.linear = skAccSt.getSteering(personaje).linear;
        st.angular = face.getSteering(personaje).angular;
        return st;
    }
}
