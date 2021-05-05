using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAlignSD : SteeringBehaviour
{
    protected new bool finishedLinear { get { return true; } }
    private AlignSteering alSt = new AlignSteering();

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        float accOrientation = 0;
        personaje.group = SimulationManager.PersonajesCerca(personaje);
        foreach (PersonajeBase person in personaje.group)
        {
            accOrientation += person.orientacion;
        }
        personaje.fakeAlign.rotacion = accOrientation / personaje.group.Count;
        personaje.fakeAlign.transform.eulerAngles = new Vector3(0,(accOrientation / personaje.group.Count)*Bodi.RadianesAGrados, 0);
        alSt.target = personaje.fakeAlign;
        Steering st = alSt.getSteering(personaje);

        _finishedAngular = alSt.finishedAngular;
        return st;
    }
}
