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
        velocidad = Vector3.zero; //informar a la peña, SINO --> Se mantiene el vector de velocidad de la task anterior
        kinetic.Clear();
        kinetic.Add(st);
    }
}
