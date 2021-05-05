using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajeNPC : PersonajeBase
{
    internal override void addTask(SteeringBehaviour st)
    {
        throw new System.NotImplementedException();
    }

    internal override void disband()
    {
        throw new System.NotImplementedException();
    }

    internal override void newTask(SteeringBehaviour st)
    {
        kinetic.Add(st);
    }
}
