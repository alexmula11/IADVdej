using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormacionGridSD : SteeringBehaviour
{
    private Vector3 offsetPosition;
    private float offsetOrientation;
    private PathFollowEndSD followPath;
    private AlignSteering faceSD = new AlignSteering();
    private Vector2 lastDestiny;

    public FormacionGridSD(Vector3 offsetPosition, float offsetOrientation)
    {
        this.offsetPosition = offsetPosition;
        this.offsetOrientation = offsetOrientation;
    }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        _finishedLinear = _finishedAngular = false;
        Vector3 newOffset = SimulationManager.DirectionToVector(_target.orientacion + SimulationManager.VectorToDirection(offsetPosition)) * offsetPosition.magnitude;
        Vector2 destino = SimManagerFinal.positionToGrid(_target.posicion+newOffset);
        if (destino != lastDestiny)
        {
            Vector2 origen = SimManagerFinal.positionToGrid(personaje.posicion);
            List<Vector3> recorrido = SimManagerFinal.aStarPathV3(origen, destino, personaje.tipo);
            lastDestiny = destino;
            followPath = new PathFollowEndSD(recorrido);
        }
        if (followPath.finishedLinear)
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
            personaje.fakeAlign.transform.eulerAngles = new Vector3(0, (_target.orientacion + offsetOrientation) * Bodi.RadianesAGrados, 0);
            faceSD.target = personaje.fakeAlign;
            return faceSD.getSteering(personaje);
        }
        else return followPath.getSteering(personaje);
    }
}
