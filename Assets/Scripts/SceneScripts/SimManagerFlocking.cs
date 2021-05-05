using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimManagerFlocking : SimulationManager
{
    private new void Start()
    {
        PersonajePlayer[] equipillo = GameObject.FindObjectsOfType<PersonajePlayer>();
        charactersInTeam = new List<PersonajeBase>(equipillo);
        int space = 20;
        System.Random random = new System.Random();
        int lider = random.Next(charactersInTeam.Count);
        for (int i=0; i < charactersInTeam.Count; i++)
        {
            charactersInTeam[i].transform.position = new Vector3(-50 + i%5 * space, 0, -50 + (int)(i/5) * space);
            charactersInTeam[i].posicion = new Vector3(-50 + i % 5 * space, 0, -50 + (int)(i / 5) * space);
            if (i != lider)
            {
                FlockingSD flocking = new FlockingSD();
                flocking.target = charactersInTeam[lider];
                charactersInTeam[i].newTask(flocking);
            }
        }
        (charactersInTeam[lider] as PersonajePlayer).newTask(new WanderSD(2 * (float)System.Math.PI, 5, 30 * Bodi.GradosARadianes, 2));
        (charactersInTeam[lider] as PersonajePlayer).selected = true;
        characterWithFocus = charactersInTeam[lider];
        ui.showDebugInfo(true);
    }
}
