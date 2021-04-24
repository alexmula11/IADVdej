using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajePlayer : PersonajeBase
{
    [SerializeField]
    private Behaviour halo;


    internal bool selected { get { return halo.enabled; } set { halo.enabled = value; } }

    internal override void newTask(SteeringBehaviour st)
    {
        kinetic.Clear();
        kinetic.Add(new WallAvoidance3WhiswersSD());
        kinetic.Add(st);
    }
}
