using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajePlayer : PersonajeBase
{
    [SerializeField]
    private Behaviour halo;



    internal bool selected { get { return halo.enabled; } set { halo.enabled = value; foreach (GameObject gizmo in gizmosGOs) gizmo.SetActive(value); } }

    internal override void addTask(SteeringBehaviour st)
    {
        List<SteeringBehaviour> newTasks = new List<SteeringBehaviour>();
        newTasks.Add(new WallAvoidance3WhiswersSD());
        newTasks.Add(st);
        newTasks.AddRange(kinetic.GetRange(1,kinetic.Count-1));
        kinetic.Clear();
        kinetic = new List<SteeringBehaviour>(newTasks);
    }

    internal override void disband()
    {
        formacion = null;
        kinetic.Clear();
        kinetic.Add(new WallAvoidance3WhiswersSD());
        kinetic.Add(new WanderSD(2 * (float)System.Math.PI, 5, 30 * GradosARadianes, 2));
    }

    internal override void newTask(SteeringBehaviour st)
    {
        kinetic.Clear();
        kinetic.Add(new WallAvoidance3WhiswersSD());
        kinetic.Add(st);
    }

    internal override void newTaskLowWA(SteeringBehaviour st)
    {
        kinetic.Clear();
        WallAvoidance3WhiswersSD wall = new WallAvoidance3WhiswersSD();
        wall.mult = 0.5f;
        kinetic.Add(wall);
        kinetic.Add(st);
    }

    internal override void newTaskWOWA(SteeringBehaviour st)
    {
        kinetic.Clear();
        kinetic.Add(st);
    }

}
