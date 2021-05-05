using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour
{
    [SerializeField]
    protected PersonajeBase _target;
    internal protected PersonajeBase target { get { return _target; } set { _target = value; } }

    protected bool _finishedLinear=false, _finishedAngular=false;
    protected internal bool finishedLinear { get { return _finishedLinear; } }
    protected internal bool finishedAngular { get { return _finishedAngular; } }

    internal protected abstract Steering getSteering(PersonajeBase personaje);
}
