using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour
{
    [SerializeField]
    protected PersonajeBase _target;
    internal protected PersonajeBase target { get { return _target; } set { _target = value; } }

    protected bool _finished=false;
    protected internal bool finished { get { return _finished; } }

    internal protected abstract Steering getSteering(PersonajeBase personaje);
}
