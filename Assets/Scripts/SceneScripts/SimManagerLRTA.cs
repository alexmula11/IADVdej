using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimManagerLRTA : SimulationManager
{
    [SerializeField]
    private PersonajeBase personajeBase;

    [SerializeField]
    private GameObject[] wallFathers;

    private bool[][] muros = new bool[50][];

    internal const int BLOCKSIZE = 4;
    
    private new void Start()
    {
        for (int i=0; i<muros.Length;i++)
        {
            muros[i] = new bool[25];
        }

        foreach (GameObject wall in wallFathers)
        {
            muros[(int)wall.transform.position.x / BLOCKSIZE + muros.Length/2][(int)wall.transform.position.z / BLOCKSIZE + muros[0].Length / 2] = true;
        }
    }

    protected new void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 9))
            {
                return;
            }
            else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 10))
            {
                LRTASD lrtaSteering = new LRTASD(muros,
                    new Vector2((int)personajeBase.posicion.x / SimManagerLRTA.BLOCKSIZE, (int)personajeBase.posicion.z / SimManagerLRTA.BLOCKSIZE),
                    new Vector2((int)hit.point.x / SimManagerLRTA.BLOCKSIZE, (int)hit.point.z / SimManagerLRTA.BLOCKSIZE));
                personajeBase.newTaskWOWA(lrtaSteering);
            }
        }
    }

}
