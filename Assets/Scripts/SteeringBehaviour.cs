using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour
{
    [SerializeField]
    protected PersonajeBase _target;
    internal PersonajeBase target { get { return _target; } set { _target = value; } }

    internal enum MOV_TYPE { UNIFORM, ACCELERATED}
    internal MOV_TYPE steeringType = MOV_TYPE.UNIFORM;

    internal abstract Steering getSteering(PersonajeBase personaje);
}
