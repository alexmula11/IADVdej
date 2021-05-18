using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    protected static List<PersonajeBase> charactersInTeam;
    [SerializeField]
    protected UIManager ui;
    [SerializeField]
    private FormationManager formationManager;


    internal static readonly float GROUP_STEERING_RADIUS = 200;

    private HashSet<PersonajeBase> selectedUnits = new HashSet<PersonajeBase>();
    protected PersonajeBase characterWithFocus;


    [SerializeField]
    private GameObject routeMarkPrefab, routeLinePrefab;
    private List<Vector3> pathToSet = new List<Vector3>();


    private enum MOUSE_ACTION
    {
        SELECT = 0,
        MOVE = 1,
        ROUTE_SET = 2,
        FORM_SET = 3
    }
    private MOUSE_ACTION mouseBehav = 0;
    protected bool mouseOverUI=false;

    protected void Start()
    {
        PersonajePlayer[] equipillo = GameObject.FindObjectsOfType<PersonajePlayer>();
        charactersInTeam = new List<PersonajeBase>(equipillo);
    }


    protected void Update()
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
                                if (person.currentFormacion != null)
                                {
                                    formationManager.removeFormacion(person.currentFormacion);
                                    person.currentFormacion.disband();
                                }
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
                                if (person.currentFormacion != null && person.currentFormacion.lider!=person)
                                {
                                    formationManager.removeFormacion(person.currentFormacion);
                                    person.currentFormacion.disband();
                                }
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
                            setMouseBehaviour(0);
                            ui.selectMouseOption(0);
                            foreach (PersonajeBase person in selectedUnits)
                            {
                                if (person.currentFormacion != null)
                                {
                                    formationManager.removeFormacion(person.currentFormacion);
                                    person.currentFormacion.disband();
                                }
                            }
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
            else if (mouseBehav == MOUSE_ACTION.FORM_SET)
            {
                if (selectedUnits.Count > 0)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 8))
                        {
                            /* PARA FORMACIONES
                            foreach (PersonajeBase person in selectedUnits)
                            {
                                if (person.currentFormacion != null)
                                {
                                    formationManager.removeFormacion(person.currentFormacion);
                                    person.currentFormacion.disband();
                                }
                            }
                            //Asignamos el lider que clicamos
                            PersonajeBase lider = hit.collider.gameObject.GetComponent<PersonajeBase>();
                            Formacion formacion = new FormacionCuadrado(lider);
                            foreach (PersonajeBase person in selectedUnits)
                            {
                                person.currentFormacion = formacion;
                                if (person != lider)
                                {
                                    formacion.addMiembro(person);
                                }
                            }
                            formacion.formacionASusPuestos();
                            formationManager.addFormation(formacion);
                            */

                            //PARA FLOCKING
                            PersonajeBase lider = hit.collider.gameObject.GetComponent<PersonajeBase>();
                            foreach (PersonajeBase person in selectedUnits)
                            {
                                if (person!= lider)
                                {
                                    FlockingSD flocking = new FlockingSD();
                                    flocking.target = lider;
                                    person.newTask(flocking);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    




    protected void FixedUpdate()
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

    internal static List<PersonajeBase> PersonajesCerca(PersonajeBase person)
    {
        List<PersonajeBase> personsCerca = new List<PersonajeBase>();
        foreach (PersonajeBase personCerca in charactersInTeam)
        {
            if (personCerca != person)
            {
                if ((person.posicion - personCerca.posicion).magnitude < GROUP_STEERING_RADIUS)
                {
                    personsCerca.Add(personCerca);
                }
            }
        }
        return personsCerca;
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
