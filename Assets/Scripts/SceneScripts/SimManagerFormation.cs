using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimManagerFormation : SimulationManager
{

    private List<Formacion> formaciones = new List<Formacion>();

    protected enum MOUSE_ACTION_FORMATION
    {
        SELECT = 0,
        MOVE = 1,
        TRIANGLE_FORMATION = 2,
        SQUARE_FORMATION = 3,
        ROLE_FORMATION = 4
    }
    protected new MOUSE_ACTION_FORMATION mouseBehav;

    internal override void setMouseBehaviour(int behaviour)
    {
        mouseBehav = (MOUSE_ACTION_FORMATION)behaviour;
    }
    

    protected new void Start()
    {
        base.Start();
        foreach (PersonajeBase person in charactersInScene)
        {
            person.newTask(new WanderSD(2 * (float)System.Math.PI, 5, 30 * Bodi.GradosARadianes, 2));
        }
    }

    protected new void Update()
    {
        if (!mouseOverUI)
        {
            if (mouseBehav == MOUSE_ACTION_FORMATION.SELECT)
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
            else if (mouseBehav == MOUSE_ACTION_FORMATION.MOVE)
            {
                if (selectedUnits.Count > 0)
                {
                    if (Input.GetMouseButton(0))
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 8))
                        {
                            PersonajeBase personajeObjetivo = hit.collider.gameObject.GetComponent<PersonajeBase>();
                            foreach (PersonajeBase person in selectedUnits)
                            {
                                if (person.currentFormacion != null)
                                {
                                    formaciones.Remove(person.currentFormacion);
                                    person.currentFormacion.disband();
                                }
                                PursueSD pursueSD = new PursueSD();
                                pursueSD.target = personajeObjetivo;
                                /*person.fake.posicion = hit.point;
                                 * person.fake.moveTo(hit.point);
                                 * person.fake.innerDetector = person.innerDetector;*/
                                person.newTask(pursueSD);
                            }
                        }
                        else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 10))
                        {
                            foreach (PersonajeBase person in selectedUnits)
                            {
                                if (person.currentFormacion != null && person.currentFormacion.lider != person)
                                {
                                    formaciones.Add(person.currentFormacion);
                                    person.currentFormacion.disband();
                                }
                                //person.setAction(new AgentActionMove(new Vector2(hit.point.x, hit.point.z), person.innerDetector, person.outterDetector));
                                PursueSD pursueSD = new PursueSD();
                                pursueSD.target = person.fakeMovement;
                                person.fakeMovement.posicion = hit.point;
                                person.fakeMovement.moveTo(hit.point); // mover los fakes para debuggeo
                                person.fakeMovement.innerDetector = person.innerDetector;
                                person.newTask(pursueSD);
                            }
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 8))
                    {
                        foreach (PersonajeBase person in selectedUnits)
                        {
                            if (person.currentFormacion != null)
                            {
                                formaciones.Remove(person.currentFormacion);
                                person.currentFormacion.disband();
                            }
                        }
                        //Asignamos el lider que clicamos
                        PersonajeBase lider = hit.collider.gameObject.GetComponent<PersonajeBase>();
                        Formacion formacion = null;
                        if (mouseBehav == MOUSE_ACTION_FORMATION.TRIANGLE_FORMATION)
                        {
                            formacion = new FormacionTriangulo(lider);
                        }
                        else if (mouseBehav == MOUSE_ACTION_FORMATION.SQUARE_FORMATION)
                        {
                            formacion = new FormacionCuadrado(lider);
                        }
                        else if (mouseBehav == MOUSE_ACTION_FORMATION.ROLE_FORMATION)
                        {
                            formacion = new FormacionPorRoles(lider);
                        }
                        foreach (PersonajeBase person in selectedUnits)
                        {
                            person.currentFormacion = formacion;
                            if (person != lider)
                            {
                                formacion.addMiembro(person);
                            }
                        }
                        formacion.formacionASusPuestos();
                        formaciones.Add(formacion);
                    }
                }
            }
        }
    }


    private new void FixedUpdate()
    {
        if (characterWithFocus != null)
        {
            ui.actualizeAgentDebugInfo(characterWithFocus);
        }
        for (int i = 0; i < formaciones.Count; i++)
        {
            formaciones[i].checkWaitForFormation();
        }
    }
}
