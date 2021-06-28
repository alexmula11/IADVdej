using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAndFaceSD : SteeringBehaviour
{
    FaceSD face  = new FaceSD();
    StopSteering stop = new StopSteering();
    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        face.target = _target;
        Steering st = new Steering();
        st.angular = face.getSteering(personaje).angular;
        st.linear = stop.getSteering(personaje).linear;

        _finishedLinear = stop.finishedLinear;
        _finishedAngular = face.finishedAngular;
        return st;
    }
}
