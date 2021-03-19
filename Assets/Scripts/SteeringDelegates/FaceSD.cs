using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceSD : SteeringBehaviour
{
    private AlignSteering alsteer = new AlignSteering();
    internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        Vector3 direction = _target.posicion - personaje.posicion;

        //comprobamos direccion 0
        if(direction.magnitude == 0)
        {
            st.linear =  Vector3.zero;
            st.angular = 0;
            return st;
        }
        personaje.fake.posicion = _target.posicion;

        alsteer.target = personaje.fake;
        alsteer.target.orientacion = (float)System.Math.Atan2(-alsteer.target.posicion.x,alsteer.target.posicion.z);

        return alsteer.getSteering(personaje);
    }
}
