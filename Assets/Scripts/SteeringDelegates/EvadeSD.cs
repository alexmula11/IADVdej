using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeSD : SteeringBehaviour
{
    private FaceSD face = new FaceSD();
    //private LeaveSteering skAccSt = new LeaveSteering();
    private FleeSteering skAccSt = new FleeSteering();

    internal new PersonajeBase target { get { return _target; } set { _target = value; face.target = value; skAccSt.target = value; } }

    internal protected override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        st.linear = skAccSt.getSteering(personaje).linear;
        st.angular = face.getSteering(personaje).angular;
        if (skAccSt.finished && face.finished)
        {
            _finished = true;
        }
        return st;
    }
}
