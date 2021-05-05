using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceSD : SteeringBehaviour
{
    protected new bool finishedLinear { get { return true; } }

    private AlignSteering alsteer = new AlignSteering();

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        //personaje.fakeAlign.orientacion = (float)System.Math.Atan2(-_target.posicion.x, _target.posicion.z);
        personaje.fakeAlign.orientacion = SimulationManager.VectorToDirection(_target.posicion - personaje.posicion);
        if (personaje.fakeAlign.orientacion > System.Math.PI)
        {
            personaje.fakeAlign.orientacion -= 2 * (float)System.Math.PI;
        }else if (personaje.fakeAlign.orientacion < -System.Math.PI)
        {
            personaje.fakeAlign.orientacion += 2 * (float)System.Math.PI;
        }
        personaje.fakeAlign.transform.eulerAngles = new Vector3(0, personaje.fakeAlign.orientacion * Bodi.RadianesAGrados, 0);
        alsteer.target = personaje.fakeAlign;
        Steering st = alsteer.getSteering(personaje);
        st.linear = Vector3.zero;
        //comprobamos direccion 0
        _finishedAngular = st.angular == 0;
        return st;
    }
}
