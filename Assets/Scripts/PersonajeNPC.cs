using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajeNPC : PersonajeBase
{
    internal override void newTask(SteeringBehaviour st)
    {
        kinetic.Add(st);
    }
}
