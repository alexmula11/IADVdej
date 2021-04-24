using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAvoidance3WhiswersSD : SteeringBehaviour
{
    private PursueSD pursueSD = new PursueSD();
    private float secondaryWhiskersAngle, secondaryWhiskersLength, primaryWhiskerLenght, wallOffset;
    protected new bool _finished = true;
    protected internal new bool finished { get { return _finished; } }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        secondaryWhiskersAngle = personaje.outterAngleVision;
        secondaryWhiskersLength = personaje.velocidad.magnitude;
        primaryWhiskerLenght = personaje.velocidad.magnitude*2;
        //secondaryWhiskersLength = personaje.maxMovSpeed / 2;
        //primaryWhiskerLenght = personaje.maxMovSpeed;
        wallOffset = personaje.innerDetector;

        RaycastHit leftWHit, rightWHit, midWHit;
        float leftOri = personaje.orientacion - secondaryWhiskersAngle;
        if (leftOri > System.Math.PI)
            leftOri -= 2 * (float)System.Math.PI;
        else if (leftOri < -System.Math.PI)
            leftOri += 2 * (float)System.Math.PI;
        float rightOri = personaje.orientacion + secondaryWhiskersAngle;
        if (rightOri > System.Math.PI)
            rightOri -= 2 * (float)System.Math.PI;
        else if (rightOri < -System.Math.PI)
            rightOri += 2 * (float)System.Math.PI;
        bool midWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(personaje.orientacion), out midWHit, primaryWhiskerLenght, 1 << 9);
        bool leftWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(leftOri), out leftWHit, secondaryWhiskersLength, 1 << 9);
        bool rightWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(SimulationManager.VectorToDirection(personaje.velocidad) + secondaryWhiskersAngle), out rightWHit, secondaryWhiskersLength, 1 << 9);

        if (midWhisker)
        {
            _finished = false;

            Vector3 newPos = midWHit.point + midWHit.normal.normalized * wallOffset;

            newPos = new Vector3(newPos.x,0,newPos.z);
            personaje.fakeAvoid.posicion = newPos;
            personaje.fakeAvoid.innerDetector = personaje.innerDetector;
            personaje.fakeAvoid.moveTo(newPos);

        }else if (leftWhisker && !rightWhisker)
        {
            _finished = false;

            float hipotenusa = leftWHit.distance;
            float transversalDistance = hipotenusa * (float)System.Math.Sin(secondaryWhiskersAngle);
            float longitudinalDistance = hipotenusa * (float)System.Math.Cos(secondaryWhiskersAngle);

            //CUSTOM FOR INNER CORNERS
            Vector3 newPos = personaje.posicion + personaje.velocidad.normalized*longitudinalDistance 
                + SimulationManager.DirectionToVector(personaje.orientacion + 90 * Bodi.GradosARadianes)*(wallOffset-transversalDistance);

            newPos = new Vector3(newPos.x, 0, newPos.z);
            personaje.fakeAvoid.posicion = newPos;
            personaje.fakeAvoid.innerDetector = personaje.innerDetector;
            personaje.fakeAvoid.moveTo(newPos);

        }
        else if (rightWhisker && !leftWhisker)
        {
            _finished = false;

            float hipotenusa = rightWHit.distance;
            float transversalDistance = hipotenusa * (float)System.Math.Sin(secondaryWhiskersAngle);
            float longitudinalDistance = hipotenusa * (float)System.Math.Cos(secondaryWhiskersAngle);

            //CUSTOM FOR INNER CORNERS
            Vector3 newPos = personaje.posicion + personaje.velocidad.normalized * longitudinalDistance
                + SimulationManager.DirectionToVector(personaje.orientacion - 90 * Bodi.GradosARadianes) * (wallOffset - transversalDistance);

            newPos = new Vector3(newPos.x, 0, newPos.z);
            personaje.fakeAvoid.posicion = newPos;
            personaje.fakeAvoid.innerDetector = personaje.innerDetector;
            personaje.fakeAvoid.moveTo(newPos);
            
        }
        else if (_finished || (!_finished && pursueSD.finished))
        {
            _finished = true;
            return new Steering();
        }
        pursueSD.target = personaje.fakeAvoid;
        return pursueSD.getSteering(personaje);
    }
}
