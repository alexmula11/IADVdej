using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormacionSD : SteeringBehaviour
{
    private Vector3 offsetPosition;
    private float offsetOrientation;
    private OffsetPursuitSD opSD = new OffsetPursuitSD();
    private AlignSteering faceSD = new AlignSteering();

    public FormacionSD(Vector3 offsetPosition, float offsetOrientation)
    {
        this.offsetPosition = offsetPosition;
        this.offsetOrientation = offsetOrientation;
    }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        _finishedLinear = _finishedAngular = false;
        Vector3 newOffset = SimulationManager.DirectionToVector(_target.orientacion + SimulationManager.VectorToDirection(offsetPosition))*offsetPosition.magnitude;
        opSD.target = _target;
        opSD.offset = newOffset;
        Steering st = opSD.getSteering(personaje);
        if (opSD.finishedLinear)
        {
            personaje.fakeAlign.orientacion = _target.orientacion + offsetOrientation;
            if (personaje.fakeAlign.orientacion < -System.Math.PI)
            {
                personaje.fakeAlign.orientacion += 2 * (float)System.Math.PI;
            }
            else if (personaje.fakeAlign.orientacion > System.Math.PI)
            {
                personaje.fakeAlign.orientacion -= 2 * (float)System.Math.PI;
            }
            personaje.fakeAlign.transform.eulerAngles = new Vector3(0, (_target.orientacion + offsetOrientation)*Bodi.RadianesAGrados,0);
            faceSD.target = personaje.fakeAlign;
            st.angular = faceSD.getSteering(personaje).angular;
        }
        //_finishedLinear = opSD.finishedLinear;
        //_finishedAngular = faceSD.finishedAngular;
        return st;
    }
}
