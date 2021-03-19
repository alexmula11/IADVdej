using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering
{
    internal bool finished = false;
    /*protected PersonajeBase owner;
    internal void setAgent(PersonajeBase owner)
    {
        this.owner = owner;
    }*/
    private Vector3 _linear;
    private float _angular;
    internal Vector3 linear { get { return _linear; } set { _linear = value; } }
    internal float angular { get { return _angular; } set { _angular = value; } } 

}
