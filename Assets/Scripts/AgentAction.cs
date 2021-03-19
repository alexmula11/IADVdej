using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentAction
{
    internal enum ACTION
    {
        STAY, MOVE, FOLLOW, ATTACK
    }
    internal ACTION accion = ACTION.STAY;
    internal ACTION action { get { return accion; } }
}
