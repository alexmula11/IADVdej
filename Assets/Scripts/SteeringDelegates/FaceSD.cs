using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceSD : SteeringBehaviour
{
    private AlignSteering alsteer = new AlignSteering();


    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        personaje.fakeAlign.posicion = _target.posicion;
        alsteer.target = personaje.fakeAlign;
        alsteer.target.orientacion = (float)System.Math.Atan2(-alsteer.target.posicion.x, alsteer.target.posicion.z);
        Steering st = alsteer.getSteering(personaje);
        st.linear = Vector3.zero;
        //comprobamos direccion 0
        if (st.angular==0)
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
