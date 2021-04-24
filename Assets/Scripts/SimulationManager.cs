using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField]
    private UIManager ui;


    private HashSet<PersonajeBase> selectedUnits = new HashSet<PersonajeBase>();
    private PersonajeBase characterWithFocus;


    [SerializeField]
    private GameObject routeMarkPrefab, routeLinePrefab;
    private List<Vector3> pathToSet = new List<Vector3>();

    private enum MOUSE_ACTION
    {
        SELECT = 0,
        MOVE = 1,
        ROUTE_SET = 2
    }
    private MOUSE_ACTION mouseBehav = 0;
    private bool mouseOverUI=false;


    private void Update()
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
                    else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    {
                        if (Input.GetMouseButton(0))
                        {
                            RaycastHit hit;
                            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 8))
                            {
                                PersonajeBase character = hit.collider.gameObject.GetComponent<PersonajeBase>();
                                characterWithFocus = character;
                                ui.actualizeAgentDebugInfo(character);
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
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 8))
                        {
                            PersonajeBase personajeObjetivo = hit.collider.gameObject.GetComponent<PersonajeBase>();
                            foreach (PersonajeBase person in selectedUnits)
                            {
                                person.setAction(new AgentActionMove(new Vector2(hit.point.x, hit.point.z), person.innerDetector, person.outterDetector));
                                PursueSD pursueSD = new PursueSD();
                                pursueSD.target = personajeObjetivo;
                                /*person.fake.posicion = hit.point;
                                person.fake.moveTo(hit.point);
                                person.fake.innerDetector = person.innerDetector;*/
                                person.newTask(pursueSD);
                            }
                        }
                        else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 10))
                        {
                            foreach (PersonajeBase person in selectedUnits)
                            {
                                person.setAction(new AgentActionMove(new Vector2(hit.point.x,hit.point.z),person.innerDetector,person.outterDetector));
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
            else if (mouseBehav == MOUSE_ACTION.ROUTE_SET)
            {
                if (selectedUnits.Count > 0)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 11))
                        {
                            setRouteOnUnits();
                            pathToSet.Clear();
                        }
                        else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 10))
                        {
                            if (pathToSet.Count == 0)
                            {
                                foreach (PersonajeBase unit in selectedUnits)
                                {
                                    foreach (Transform routeElem in unit.routeMarks)
                                    {
                                        Destroy(routeElem);
                                    }
                                }
                            }
                            pathToSet.Add(hit.point);
                            foreach (PersonajeBase unit in selectedUnits)
                            {
                                GameObject routeMark = Instantiate(routeMarkPrefab, unit.routeMarks);
                                routeMark.transform.position = new Vector3(hit.point.x, 0.05f, hit.point.z);
                                if (pathToSet.Count > 1)
                                {
                                    GameObject routeLine = Instantiate(routeLinePrefab, unit.routeMarks);
                                    routeLine.GetComponent<FlechaDeRutaDelegate>().setRouteDirection(pathToSet[pathToSet.Count - 2], pathToSet[pathToSet.Count - 1]);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    




    private void FixedUpdate()
    {
        if (characterWithFocus != null)
        {
            ui.actualizeAgentDebugInfo(characterWithFocus);
        }
    }

    internal void setMouseOverUI(bool over)
    {
        mouseOverUI = over;
    }


    internal void setMouseBehaviour(int behaviour)
    {
        mouseBehav = (MOUSE_ACTION)behaviour;
        if (mouseBehav != MOUSE_ACTION.ROUTE_SET && pathToSet.Count>0)
        {
            setRouteOnUnits();
            pathToSet.Clear();
        }

    }


    private void setRouteOnUnits()
    {
        if (pathToSet.Count == 1)
        {
            foreach (PersonajeBase unit in selectedUnits)
            {
                List<Vector3> puntosDeRuta = new List<Vector3>(pathToSet);
                GameObject mark = Instantiate(routeMarkPrefab, unit.routeMarks);
                mark.transform.position = unit.posicion;
                puntosDeRuta.Add(unit.posicion);
                unit.newTask(new PathFollowingNOPathOffsetSD(puntosDeRuta));
            }
        }
        else
        {
            foreach (PersonajeBase unit in selectedUnits)
            {
                List<Vector3> puntosDeRuta = new List<Vector3>(pathToSet);
                unit.newTask(new PathFollowingNOPathOffsetSD(puntosDeRuta));
            }
        }
    }






    internal static float VectorToDirection(Vector3 direction)
    {
            return (float)(System.Math.Atan2(direction.x , direction.z));
    }
    internal static Vector3 DirectionToVector(float direction)
    {
        //return new Vector3((float)System.Math.Cos(direction), 0,(float)System.Math.Sin(direction));
        return new Vector3((float)System.Math.Sin(direction), 0, (float)System.Math.Cos(direction));
    }

    internal static float TurnAmountInDirection(float originAngle, float destAngle)
    {
        if (originAngle <= 0)
        {
            if (destAngle >= 0)
            {
                if (destAngle - System.Math.PI >= originAngle)
                {
                    return (float)((-System.Math.PI - originAngle) - (System.Math.PI - destAngle));
                }
                else
                {
                    return destAngle + (-originAngle);
                }
            }
            else
            {
                return destAngle - originAngle;
            }
        }
        else
        {
            if (destAngle >= 0)
            {
                return destAngle - originAngle;
            }
            else
            {
                if (destAngle + System.Math.PI >= originAngle)
                {
                    return -originAngle + destAngle;
                }
                else
                {
                    return (float)((System.Math.PI - originAngle) + (destAngle + System.Math.PI));
                }
            }
        }
    }
}
