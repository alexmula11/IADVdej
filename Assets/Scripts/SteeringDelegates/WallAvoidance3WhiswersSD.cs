using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAvoidance3WhiswersSD : SteeringBehaviour
{
    private PursueSD pursueSD = new PursueSD();
    private float secondaryWhiskersAngle, secondaryWhiskersLength, primaryWhiskerLenght, wallOffset;
    protected new bool _finishedLinear=true, _finishedAngular = true;
    protected internal new bool finishedLinear { get { return _finishedLinear; } }
    protected internal new bool finishedAngular { get { return _finishedAngular; } }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        secondaryWhiskersAngle = personaje.outterAngleVision;
        secondaryWhiskersLength = personaje.velocidad.magnitude;
        primaryWhiskerLenght = personaje.velocidad.magnitude*2.5f;
        //secondaryWhiskersLength = personaje.maxMovSpeed / 2;
        //primaryWhiskerLenght = personaje.maxMovSpeed;
        wallOffset = personaje.innerDetector*1.5f;

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
        bool midWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(personaje.orientacion), out midWHit, primaryWhiskerLenght, 1 << 9 | 1 << 8);
        bool leftWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(leftOri), out leftWHit, secondaryWhiskersLength, 1 << 9 | 1 << 8);
        bool rightWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(rightOri), out rightWHit, secondaryWhiskersLength, 1 << 9 | 1 << 8);
        

        if (midWhisker)
        {
            _finishedLinear = _finishedAngular = false;

            Vector3 newPos = midWHit.point + midWHit.normal.normalized * wallOffset;

            newPos = new Vector3(newPos.x,0,newPos.z);
            personaje.fakeAvoid.posicion = newPos;
            personaje.fakeAvoid.innerDetector = personaje.innerDetector;
            personaje.fakeAvoid.moveTo(newPos);

        }else if (leftWhisker && !rightWhisker)
        {
            _finishedLinear = _finishedAngular = false;

            float hipotenusa = leftWHit.distance;
            float transversalDistance = hipotenusa * (float)System.Math.Sin(secondaryWhiskersAngle);
            float longitudinalDistance = hipotenusa * (float)System.Math.Cos(secondaryWhiskersAngle);

            float transversalOri = personaje.orientacion + 90 * Bodi.GradosARadianes;
            if (transversalOri > (float)System.Math.PI)
            {
                transversalOri -= 2 * (float)System.Math.PI;
            }else if (transversalOri < (float)System.Math.PI)
            {
                transversalOri += 2 * (float)System.Math.PI;
            }
            //CUSTOM FOR INNER CORNERS
            Vector3 newPos = personaje.posicion + personaje.velocidad.normalized*longitudinalDistance 
                + SimulationManager.DirectionToVector(transversalOri)*(wallOffset-transversalDistance);

            newPos = new Vector3(newPos.x, 0, newPos.z);
            personaje.fakeAvoid.posicion = newPos;
            personaje.fakeAvoid.innerDetector = personaje.innerDetector;
            personaje.fakeAvoid.moveTo(newPos);

        }
        else if (rightWhisker && !leftWhisker)
        {
            _finishedLinear = _finishedAngular = false;

            float hipotenusa = rightWHit.distance;
            float transversalDistance = hipotenusa * (float)System.Math.Sin(secondaryWhiskersAngle);
            float longitudinalDistance = hipotenusa * (float)System.Math.Cos(secondaryWhiskersAngle);


            float transversalOri = personaje.orientacion - 90 * Bodi.GradosARadianes;
            if (transversalOri > (float)System.Math.PI)
            {
                transversalOri -= 2 * (float)System.Math.PI;
            }
            else if (transversalOri < (float)System.Math.PI)
            {
                transversalOri += 2 * (float)System.Math.PI;
            }

            //CUSTOM FOR INNER CORNERS
            Vector3 newPos = personaje.posicion + personaje.velocidad.normalized * longitudinalDistance
                + SimulationManager.DirectionToVector(transversalOri) * (wallOffset - transversalDistance);

            newPos = new Vector3(newPos.x, 0, newPos.z);
            personaje.fakeAvoid.posicion = newPos;
            personaje.fakeAvoid.innerDetector = personaje.innerDetector;
            personaje.fakeAvoid.moveTo(newPos);
            
        }
        else if (_finishedLinear || (!_finishedLinear && pursueSD.finishedLinear))
        {
            _finishedLinear = _finishedAngular = true;
            return new Steering();
        }
        pursueSD.target = personaje.fakeAvoid;
        return pursueSD.getSteering(personaje);
    }
}
