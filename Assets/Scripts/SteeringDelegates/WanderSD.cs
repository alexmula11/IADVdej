using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderSD : SteeringBehaviour
{
    protected float rotationLimit;
    protected float offset;
    protected System.Random randomizer = new System.Random(); //Hay que inicializar el random aquí, si no no es true random
    protected PursueSD pursueSD = new PursueSD();

    protected float wanderLimit;
    protected float wanderRadius;

    protected bool setup = true;

    public WanderSD(float rotationLimit, float offset, float wanderLimit, float wanderRadius)
    {
        this.rotationLimit = rotationLimit;
        this.offset = offset;
        this.wanderLimit = wanderLimit;
        this.wanderRadius = wanderRadius;
    }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        if (pursueSD.finishedLinear || setup)
        {
            setup = false;
            float angularVariation = (float)randomizer.NextDouble() * rotationLimit * 2 - rotationLimit; //rotacion random entre orientacion - limite/2 y orientacion+limite/2
            float nuevoAngulo = personaje.orientacion + angularVariation;

            Vector3 wanderCenter = personaje.posicion + SimulationManager.DirectionToVector(personaje.orientacion) * offset;
            float secondAngularVariation = (float)randomizer.NextDouble() * wanderLimit * 2 - wanderLimit;

            Vector3 wanderTarget = wanderCenter + SimulationManager.DirectionToVector(nuevoAngulo + secondAngularVariation) * wanderRadius;
            //personaje.fakeMovement.posicion = personaje.posicion + SimulationManager.DirectionToVector(nuevoAngulo) * offset;
            personaje.fakeMovement.posicion = wanderTarget;
            personaje.fakeMovement.moveTo(wanderTarget);
        }
        pursueSD.target = personaje.fakeMovement;
        return pursueSD.getSteering(personaje);
    }
}
