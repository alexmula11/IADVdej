using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimManagerFlocking : SimulationManager
{
    private float chPercent = 1, sepPercent = 1, followPercent = 1;
    private int lider;

    private new void Start()
    {
        PersonajePlayer[] equipillo = GameObject.FindObjectsOfType<PersonajePlayer>();
        charactersInScene = new List<PersonajeBase>(equipillo);
        int space = 20;
        System.Random random = new System.Random();
        lider = random.Next(charactersInScene.Count);
        for (int i=0; i < charactersInScene.Count; i++)
        {
            charactersInScene[i].transform.position = new Vector3(-50 + i%5 * space, 0, -50 + (int)(i/5) * space);
            charactersInScene[i].posicion = new Vector3(-50 + i % 5 * space, 0, -50 + (int)(i / 5) * space);
            if (i != lider)
            {
                FlockingSD flocking = new FlockingSD(chPercent,sepPercent,followPercent);
                flocking.target = charactersInScene[lider];
                charactersInScene[i].newTask(flocking);
            }
            else
            {
                charactersInScene[i].applyTipo(StatsInfo.TIPO_PERSONAJE.ARQUERO);
            }
        }
        (charactersInScene[lider] as PersonajePlayer).newTask(new WanderSD(2 * (float)System.Math.PI, 5, 30 * Bodi.GradosARadianes, 2));
        (charactersInScene[lider] as PersonajePlayer).selected = true;
        selectedUnits.Add(charactersInScene[lider]);
        characterWithFocus = charactersInScene[lider];
        ui.showDebugInfo(true);
    }


    protected new void Update()
    {
        //INPUT MANAGEMENT
        //CAMERA INPUT HERE (AROUND THE MAP)


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
        }
    }

    public void setChPercent(string percent)
    {
        if (percent != "")
            chPercent = float.Parse(percent);
    }
    public void setSepPercent(string percent)
    {
        if (percent != "")
            sepPercent = float.Parse(percent);
    }
    public void setFollowPercent(string percent)
    {
        if (percent != "")
            followPercent = float.Parse(percent);
    }

    public void applyFlockingConfiguration()
    {
        for (int i = 0; i < charactersInScene.Count; i++)
        {
            if (i != lider)
            {
                FlockingSD flocking = new FlockingSD(chPercent, sepPercent, followPercent);
                flocking.target = charactersInScene[lider];
                charactersInScene[i].newTask(flocking);
            }
        }
    }





}
