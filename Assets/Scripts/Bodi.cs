using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodi : MonoBehaviour
{
    [SerializeField]
    protected float rotationSpeed = 180f, maxMovementSpeed = 3f, movementAccel = 0.5f, mass = 1f, maxAngularAcelleration=5f;    //HOW MANY DEGREES IN A SECOND
    internal float rotSpeed { get { return rotationSpeed; } }
    internal float maxMovSpeed { get { return maxMovementSpeed; } }
    internal float movAcc { get { return movementAccel; } }
    internal float maxAngAcc { get {return maxAngularAcelleration;}}

    internal float rotacion;
    internal float orientacion;
    internal Vector3 velocidad;
    internal Vector3 aceleracion;
    internal Vector3 posicion;

    internal void moveTo(Vector3 pos)
    {
        transform.position = pos;
    }

    private void Awake()
    {
        posicion = transform.position;
    }

    //para pasar de una posicion a un angulo hacemos uso de la funcion atan2
    internal float posicionToAngulo(Vector3 pos)
    {
        return (float)(System.Math.Atan2(pos.x , pos.z) * 360 / (2 * System.Math.PI));
    }

    //Aplicamos la relacion de sin/cos a la orientacion para convertirlo en vector de direccion
    internal Vector3 orientacionToVector()
    {
        Vector3 aux = transform.position;
        aux.z = Mathf.Sin(orientacion);
        aux.x = Mathf.Cos(orientacion);
        return aux;
    }

    internal float minAnguloRotacion(Vector3 otherNPC)
    {
        float originAngle = orientacion;
        float destAngle = posicionToAngulo(otherNPC);

        if (originAngle <= 0)
        {
            if (destAngle >= 0)
            {
                if (destAngle - 180 >= originAngle)
                {
                    return (-180 - originAngle) - (180 - destAngle);
                }
                else
                {
                    return destAngle + (-originAngle);
                }
            }
            else
            {
                return destAngle - originAngle;
            }
        }
        else
        {
            if (destAngle >= 0)
            {
                return destAngle - originAngle;
            }
            else
            {
                if (destAngle + 180 >= originAngle)
                {
                    return -originAngle + destAngle;
                }
                else
                {
                    return (180 - originAngle) + (destAngle + 180);
                }
            }
        }
    }
}
