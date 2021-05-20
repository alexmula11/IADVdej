using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimManagerLRTA : SimulationManager
{
    [SerializeField]
    private PersonajeBase personajeBase;

    [SerializeField]
    private GameObject[] wallFathers;

    private static bool[][] muros = new bool[51][];

    internal const int BLOCKSIZE = 4;
    
    private new void Start()
    {
        for (int i=0; i<muros.Length;i++)
        {
            muros[i] = new bool[26];
        }
        foreach (GameObject wall in wallFathers)
        {
            foreach (Transform singleWall in wall.transform)
            {
                Vector2 coord = positionToGrid(singleWall.position);
                muros[(int)coord.x][(int)coord.y] = true;
            }
        }
        Time.timeScale = 10;
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
                    positionToGrid(personajeBase.posicion),
                    positionToGrid(hit.point));
                personajeBase.newTaskWOWA(lrtaSteering);
            }
        }
    }


    internal static Vector2 positionToGrid(Vector3 position)
    {
        int coordX = (int)System.Math.Round(position.x) / BLOCKSIZE + muros.Length / 2;
        int coordY = (int)System.Math.Round(position.z) / BLOCKSIZE + muros[0].Length / 2;
        if (position.z < 0) coordY--;
        return new Vector2(coordX, coordY);
    }

    internal static Vector3 gridToPosition(Vector2 gridPos)
    {
        int coordX = ((int)gridPos.x - muros.Length / 2)*BLOCKSIZE;
        int coordY = ((int)gridPos.y - muros[0].Length / 2)*BLOCKSIZE + BLOCKSIZE/2;
        return new Vector3(coordX, 0, coordY);
    }
}
