﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueSD : SteeringBehaviour
{
    private FaceSD face = new FaceSD();
    private SeekAcceleratedSteering skAccSt = new SeekAcceleratedSteering();
    //private ArriveSteering skAccSt = new ArriveSteering();

    internal new PersonajeBase target { get { return _target; } set { _target = value; face.target = value; skAccSt.target = value; } }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        st.linear = skAccSt.getSteering(personaje).linear;
        st.angular = face.getSteering(personaje).angular;
        if (skAccSt.finished && face.finished)
        {
            _finished = true;
        }
        else if (skAccSt.finished)
        {
            _finished = true;
        }
        else
        {
            _finished = false;
        }
        return st;
    }
}
