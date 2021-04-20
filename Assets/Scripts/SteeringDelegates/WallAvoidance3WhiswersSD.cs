using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAvoidance3WhiswersSD : SteeringBehaviour
{
    private float secondaryWhiskersAngle, secondaryWhiskersLength, primaryWhiskerLenght, wallOffset;
    public WallAvoidance3WhiswersSD(float sWhiskAngle, float sWhiskLength, float pWhiskLength, float wallOffset)
    {
        secondaryWhiskersAngle = sWhiskAngle;
        secondaryWhiskersLength = sWhiskLength;
        primaryWhiskerLenght = pWhiskLength;
        this.wallOffset = wallOffset;
    }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        RaycastHit leftWHit, rightWHit, midWHit;
        bool midWhisker = Physics.Raycast(personaje.posicion, personaje.velocidad, out midWHit, primaryWhiskerLenght, 1 << 9);
        bool leftWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(SimulationManager.VectorToDirection(personaje.velocidad) + secondaryWhiskersAngle), out leftWHit, secondaryWhiskersLength, 1 << 9);
        bool rightWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(SimulationManager.VectorToDirection(personaje.velocidad) + secondaryWhiskersAngle), out rightWHit, secondaryWhiskersLength, 1 << 9);

        if (midWhisker)
        {
            Vector3 newPos = midWHit.point + midWHit.normal * wallOffset;
            personaje.fake.posicion = newPos;
            PursueSD pursueSD = new PursueSD();
            pursueSD.target = personaje.fake;
            return pursueSD.getSteering(personaje);
        }else if (leftWhisker && !rightWhisker)
        {
            float hipotenusa = leftWHit.distance;
            float transversalDistance = hipotenusa * (float)System.Math.Sin(secondaryWhiskersAngle * Bodi.RadianesAGrados);
            float longitudinalDistance = hipotenusa * (float)System.Math.Cos(secondaryWhiskersAngle * Bodi.RadianesAGrados);
            //CUSTOM FOR INNER CORNERS
            Vector3 newPos = personaje.posicion + personaje.velocidad.normalized*longitudinalDistance 
                + SimulationManager.DirectionToVector(personaje.orientacion + 90 * Bodi.GradosARadianes)*(wallOffset-transversalDistance);
            personaje.fake.posicion = newPos;
            PursueSD pursueSD = new PursueSD();
            pursueSD.target = personaje.fake;
            return pursueSD.getSteering(personaje);
        }else if (rightWhisker && !leftWhisker)
        {
            float hipotenusa = rightWHit.distance;
            float transversalDistance = hipotenusa * (float)System.Math.Sin(secondaryWhiskersAngle * Bodi.RadianesAGrados);
            float longitudinalDistance = hipotenusa * (float)System.Math.Cos(secondaryWhiskersAngle * Bodi.RadianesAGrados);
            //CUSTOM FOR INNER CORNERS
            Vector3 newPos = personaje.posicion + personaje.velocidad.normalized * longitudinalDistance
                + SimulationManager.DirectionToVector(personaje.orientacion - 90 * Bodi.GradosARadianes) * (wallOffset - transversalDistance);
            personaje.fake.posicion = newPos;
            PursueSD pursueSD = new PursueSD();
            pursueSD.target = personaje.fake;
            return pursueSD.getSteering(personaje);
        }
        else
        {
            return new Steering();
        }

    }
}
