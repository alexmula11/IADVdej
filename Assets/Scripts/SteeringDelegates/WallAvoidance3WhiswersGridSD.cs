using UnityEngine;

public class WallAvoidance3WhiswersGridSD : SteeringBehaviour
{
    private PursueSD pursueSD = new PursueSD();
    private float secondaryWhiskersAngle,terciaryWhiskersAngle, terciaryWhiskersLength, secondaryWhiskersLength, primaryWhiskerLenght, wallOffset, multiplier=1f;
    protected new bool _finishedLinear=true, _finishedAngular = true;
    protected internal new bool finishedLinear { get { return _finishedLinear; } }
    protected internal new bool finishedAngular { get { return _finishedAngular; } }
    protected internal float mult { get { return multiplier; } set { multiplier = value; } }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        terciaryWhiskersAngle = personaje.outterAngleVision;
        secondaryWhiskersAngle = personaje.outterAngleVision/2;
        //secondaryWhiskersLength = personaje.velocidad.magnitude*1.5f;
        //primaryWhiskerLenght = personaje.velocidad.magnitude*2.5f;
        terciaryWhiskersLength = personaje.maxMovSpeed * multiplier * 0.5f;
        secondaryWhiskersLength = personaje.maxMovSpeed*0.75f* multiplier;
        primaryWhiskerLenght = personaje.maxMovSpeed*1.5f* multiplier;
        wallOffset = personaje.innerDetector*1.5f;

        RaycastHit leftSWHit, leftTWHit, rightSWHit, rightTWHit, midWHit;

        float leftSecondaryOri = personaje.orientacion - secondaryWhiskersAngle;
        if (leftSecondaryOri > System.Math.PI)
            leftSecondaryOri -= 2 * (float)System.Math.PI;
        else if (leftSecondaryOri < -System.Math.PI)
            leftSecondaryOri += 2 * (float)System.Math.PI;

        float leftTerciaryOri = personaje.orientacion - terciaryWhiskersAngle;
        if (leftTerciaryOri > System.Math.PI)
            leftTerciaryOri -= 2 * (float)System.Math.PI;
        else if (leftTerciaryOri < -System.Math.PI)
            leftTerciaryOri += 2 * (float)System.Math.PI;


        float rightSecondaryOri = personaje.orientacion + secondaryWhiskersAngle;
        if (rightSecondaryOri > System.Math.PI)
            rightSecondaryOri -= 2 * (float)System.Math.PI;
        else if (rightSecondaryOri < -System.Math.PI)
            rightSecondaryOri += 2 * (float)System.Math.PI;

        float rightTerciaryOri = personaje.orientacion + terciaryWhiskersAngle;
        if (rightTerciaryOri > System.Math.PI)
            rightTerciaryOri -= 2 * (float)System.Math.PI;
        else if (rightTerciaryOri < -System.Math.PI)
            rightTerciaryOri += 2 * (float)System.Math.PI;


        bool midWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(personaje.orientacion), out midWHit, primaryWhiskerLenght, 1 << 9 | 1 << 8 | 1 << 13);
        bool leftSWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(leftSecondaryOri), out leftSWHit, secondaryWhiskersLength, 1 << 9 | 1 << 8 | 1 << 13);
        bool rightSWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(rightSecondaryOri), out rightSWHit, secondaryWhiskersLength, 1 << 9 | 1 << 8 | 1 << 13);
        bool leftTWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(leftTerciaryOri), out leftTWHit, terciaryWhiskersLength, 1 << 9 | 1 << 8 | 1 << 13);
        bool righTWhisker = Physics.Raycast(personaje.posicion, SimulationManager.DirectionToVector(rightTerciaryOri), out rightTWHit, terciaryWhiskersLength, 1 << 9 | 1 << 8 | 1 << 13);


        if (midWhisker)
        {
            _finishedLinear = _finishedAngular = false;

            Vector3 newPos = midWHit.point + midWHit.normal.normalized * wallOffset;

            newPos = new Vector3(newPos.x,0,newPos.z);
            Vector2 newPos2d = SimManagerFinal.positionToGrid(newPos);
            if (SimManagerFinal.terrenos[(int)newPos2d.x][(int)newPos2d.y] == StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
                newPos2d = getVecinoFranqueable(newPos2d);
            newPos = SimManagerFinal.gridToPosition(newPos2d);
            personaje.fakeAvoid.posicion = newPos;
            personaje.fakeAvoid.innerDetector = personaje.innerDetector;
            personaje.fakeAvoid.moveTo(newPos);
            pursueSD.target = personaje.fakeAvoid;

        }
        else if ((leftSWhisker || leftTWhisker) && !rightSWhisker && !righTWhisker)
        {
            _finishedLinear = _finishedAngular = false;

            float hipotenusa = 0;
            float transversalDistance = 0;
            float longitudinalDistance = 0;
            if (leftSWhisker)
            {
                hipotenusa = leftSWHit.distance;
                transversalDistance = hipotenusa * (float)System.Math.Sin(secondaryWhiskersAngle);
                longitudinalDistance = hipotenusa * (float)System.Math.Cos(secondaryWhiskersAngle);
            }
            else if (leftTWhisker)
            {
                hipotenusa = leftTWHit.distance;
                transversalDistance = hipotenusa * (float)System.Math.Sin(terciaryWhiskersAngle);
                longitudinalDistance = hipotenusa * (float)System.Math.Cos(terciaryWhiskersAngle);
            }
            else
            {
                hipotenusa = (leftSWHit.distance + leftTWHit.distance) / 2;
                transversalDistance = hipotenusa * (float)System.Math.Sin((secondaryWhiskersAngle + terciaryWhiskersAngle) / 2);
                longitudinalDistance = hipotenusa * (float)System.Math.Cos((secondaryWhiskersAngle + terciaryWhiskersAngle) / 2);
            }

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
            Vector2 newPos2d = SimManagerFinal.positionToGrid(newPos);
            if (SimManagerFinal.terrenos[(int)newPos2d.x][(int)newPos2d.y] == StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
                newPos2d = getVecinoFranqueable(newPos2d);
            newPos = SimManagerFinal.gridToPosition(newPos2d);
            personaje.fakeAvoid.posicion = newPos;
            personaje.fakeAvoid.innerDetector = personaje.innerDetector;
            personaje.fakeAvoid.moveTo(newPos);
            pursueSD.target = personaje.fakeAvoid;

        }
        else if ((rightSWhisker || righTWhisker ) && !leftSWhisker && !leftTWhisker)
        {
            _finishedLinear = _finishedAngular = false;

            float hipotenusa = 0;
            float transversalDistance = 0;
            float longitudinalDistance = 0;
            if (rightSWhisker)
            {
                hipotenusa = leftSWHit.distance;
                transversalDistance = hipotenusa * (float)System.Math.Sin(secondaryWhiskersAngle);
                longitudinalDistance = hipotenusa * (float)System.Math.Cos(secondaryWhiskersAngle);
            }
            else if (righTWhisker)
            {
                hipotenusa = leftTWHit.distance;
                transversalDistance = hipotenusa * (float)System.Math.Sin(terciaryWhiskersAngle);
                longitudinalDistance = hipotenusa * (float)System.Math.Cos(terciaryWhiskersAngle);
            }
            else
            {
                hipotenusa = (leftSWHit.distance + leftTWHit.distance) / 2;
                transversalDistance = hipotenusa * (float)System.Math.Sin((secondaryWhiskersAngle + terciaryWhiskersAngle) / 2);
                longitudinalDistance = hipotenusa * (float)System.Math.Cos((secondaryWhiskersAngle + terciaryWhiskersAngle) / 2);
            }


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
            Vector2 newPos2d = SimManagerFinal.positionToGrid(newPos);
            if (SimManagerFinal.terrenos[(int)newPos2d.x][(int)newPos2d.y] == StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
                newPos2d = getVecinoFranqueable(newPos2d);
            newPos = SimManagerFinal.gridToPosition(newPos2d);
            personaje.fakeAvoid.posicion = newPos;
            personaje.fakeAvoid.innerDetector = personaje.innerDetector;
            personaje.fakeAvoid.moveTo(newPos);
            pursueSD.target = personaje.fakeAvoid;

        }
        else if (_finishedLinear || (!_finishedLinear && pursueSD.finishedLinear))
        {
            _finishedLinear = _finishedAngular = true;
            return new Steering();
        }
        return pursueSD.getSteering(personaje);
    }

    private Vector2 getVecinoFranqueable(Vector2 nodo)
    {
        Vector2 vecino = nodo;
        int i = 1;
        while (vecino == nodo)
        {
            if (SimManagerFinal.terrenos[(int)nodo.x][(int)nodo.y + i] != StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
            {
                vecino = new Vector2(nodo.x, nodo.y + i);
            }
            else if (SimManagerFinal.terrenos[(int)nodo.x][(int)nodo.y - i] != StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
            {
                vecino = new Vector2(nodo.x, nodo.y - i);
            }else if (SimManagerFinal.terrenos[(int)nodo.x + i][(int)nodo.y] != StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
            {
                vecino = new Vector2(nodo.x + i, nodo.y);
            }
            else if (SimManagerFinal.terrenos[(int)nodo.x - i][(int)nodo.y] != StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
            {
                vecino = new Vector2(nodo.x - i, nodo.y);
            }
            else if (SimManagerFinal.terrenos[(int)nodo.x + i][(int)nodo.y + i] != StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
            {
                vecino = new Vector2(nodo.x + i, nodo.y + i);
            }
            else if (SimManagerFinal.terrenos[(int)nodo.x - i][(int)nodo.y + i] != StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
            {
                vecino = new Vector2(nodo.x - i, nodo.y + i);
            }
            else if (SimManagerFinal.terrenos[(int)nodo.x + i][(int)nodo.y - i] != StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
            {
                vecino = new Vector2(nodo.x + i, nodo.y - i);
            }
            else if (SimManagerFinal.terrenos[(int)nodo.x - i][(int)nodo.y - i] != StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
            {
                vecino = new Vector2(nodo.x - i, nodo.y - i);
            }
            i++;
        }
        return vecino;
    }
}
