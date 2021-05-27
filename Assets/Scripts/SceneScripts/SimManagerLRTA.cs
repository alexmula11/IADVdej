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
        if (!mouseOverUI)
        {
            if (mouseBehav == MOUSE_ACTION.SELECT)
            {
                if (Input.GetMouseButton(0))
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 8))
                        {
                            PersonajePlayer character = hit.collider.gameObject.GetComponent<PersonajePlayer>();
                            if (!selectedUnits.Contains(character))
                            {
                                character.selected = true;
                                characterWithFocus = character;
                                selectedUnits.Add(character);
                                ui.showDebugInfo(true);
                                ui.actualizeAgentDebugInfo(character);
                            }
                            else
                            {
                                character.selected = false;
                                if (character == characterWithFocus)
                                {
                                    characterWithFocus = null;
                                    ui.showDebugInfo(false);
                                }
                                selectedUnits.Remove(character);
                            }
                        }
                    }
                    else
                    {
                        foreach (PersonajePlayer person in selectedUnits)
                        {
                            person.selected = false;
                        }
                        selectedUnits.Clear();
                        RaycastHit hit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 8))
                        {
                            PersonajePlayer character = hit.collider.gameObject.GetComponent<PersonajePlayer>();
                            character.selected = true;
                            characterWithFocus = character;
                            selectedUnits.Add(character);
                            ui.showDebugInfo(true);
                            ui.actualizeAgentDebugInfo(character);
                        }
                        else
                        {
                            if (characterWithFocus != null)
                            {
                                characterWithFocus = null;
                                ui.showDebugInfo(false);
                            }
                        }
                    }

                }
            }
            else if (mouseBehav == MOUSE_ACTION.MOVE)
            {
                if (selectedUnits.Count > 0)
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
                            foreach (PersonajeBase person in selectedUnits)
                            {
                                Vector2 posicionDestino = positionToGrid(hit.point);
                                LRTASD lrtaSteering = null;
                                switch (person.tipo)
                                {
                                    case StatsInfo.TIPO_PERSONAJE.INFANTERIA:
                                        lrtaSteering = new LRTAManhattanSD(muros, positionToGrid(personajeBase.posicion), posicionDestino);
                                        break;
                                    case StatsInfo.TIPO_PERSONAJE.ARQUERO:
                                        lrtaSteering = new LRTAEuclideSD(muros, positionToGrid(personajeBase.posicion), posicionDestino);
                                        break;
                                    case StatsInfo.TIPO_PERSONAJE.PESADA:
                                        lrtaSteering = new LRTAChevychevSD(muros, positionToGrid(personajeBase.posicion), posicionDestino);
                                        break;
                                }
                                person.fakeAvoid.transform.position = gridToPosition(posicionDestino);
                                person.newTaskWOWA(lrtaSteering);

                            }
                        }
                    }
                }
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
