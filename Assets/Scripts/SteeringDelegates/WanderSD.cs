using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderSD : SteeringBehaviour
{
    protected float rotationLimit;
    protected float offset;
    protected System.Random randomizer = new System.Random(); //Hay que inicializar el random aquí, si no no es true random
    protected PursueSD pursueSD = new PursueSD();

    public WanderSD(float rotationLimit, float offset)
    {
        this.rotationLimit = rotationLimit;
        this.offset = offset;
    }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        float angularVariation = (float)randomizer.NextDouble() * rotationLimit - rotationLimit / 2; //rotacion random entre orientacion - limite/2 y orientacion+limite/2
        float nuevoAngulo = personaje.orientacion + angularVariation;
        personaje.fakeMovement.posicion = personaje.posicion + SimulationManager.DirectionToVector(nuevoAngulo) * offset;
        pursueSD.target = personaje.fakeMovement;
        Steering st = pursueSD.getSteering(personaje);

        return st;
    }
}
