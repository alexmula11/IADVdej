using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CohesionSD : SteeringBehaviour
{
    protected new bool finishedAngular { get { return true; } }

    protected PursueSD pursueSD = new PursueSD();

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        Vector3 accPositions = personaje.posicion;
        personaje.group = SimulationManager.PersonajesCerca(personaje);
        foreach (PersonajeBase person in personaje.group)
        {
            accPositions += person.posicion;
        }
        Vector3 newPos = accPositions / (personaje.group.Count + 1) - personaje.posicion;
        personaje.fakeMovement.posicion = newPos;
        personaje.fakeMovement.moveTo(newPos);
        pursueSD.target = personaje.fakeMovement;
        _finishedLinear = pursueSD.finishedLinear;
        return pursueSD.getSteering(personaje);
    }
}
