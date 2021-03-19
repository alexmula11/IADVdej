using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentActionStay : AgentAction
{
    private float lookingTarget;
    internal float target { get { return lookingTarget; } }

    public AgentActionStay(float lookingTarget)
    {
        accion = ACTION.STAY;
        this.lookingTarget = lookingTarget;
    }
}
