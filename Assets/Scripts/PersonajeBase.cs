using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersonajeBase : Bodi
{
    [SerializeField]
    protected string nombre = "Base Character";
    [SerializeField]
    protected PersonajeNPC fakeMovementObjetive, fakeAvoidObjetive, fakeAlignObjetive;
    [SerializeField]
    protected GameObject[] gizmosGOs;
    [SerializeField]
    protected Transform routeMarkers;

    internal PersonajeNPC fakeMovement { get { return fakeMovementObjetive; } }
    internal PersonajeNPC fakeAvoid { get { return fakeAvoidObjetive; } }
    internal PersonajeNPC fakeAlign { get { return fakeAlignObjetive; } }
    internal Transform routeMarks { get { return routeMarkers; } }

    protected Formacion formacion = null;
    internal Formacion currentFormacion { get{ return formacion; } set { formacion = value; } }

    internal List<PersonajeBase> group = new List<PersonajeBase>();



    //SENSORS
    internal float innerDetector=2f, outterDetector=10f;
    internal float innerAngleVision=5*GradosARadianes, outterAngleVision=30*GradosARadianes; //30grad -- 60grad

    
    
    //TURNING MANAGE
    internal float stayLookingDirection;
    //MOVEMENT MANAGE
    internal float currentMovementSpeed { get { return velocidad.magnitude; } }



    //ACTIONS MANAGEMENT
    private LinkedList<AgentAction> actionList = new LinkedList<AgentAction>();
    internal AgentAction currentAction { get { return actionList.First.Value; } }


    protected List<SteeringBehaviour> kinetic = new List<SteeringBehaviour>();
    protected internal SteeringBehaviour selectedBehaviour = null;
    protected internal Steering steeringActual = null;




    private void Start()
    {
        orientacion = transform.eulerAngles.y * GradosARadianes;
        //actionList.AddFirst(new AgentActionStay(orientacion));
        //newTask(new WanderSD(2 * (float)System.Math.PI, 5, 30 * GradosARadianes, 2));
        //newTask(new CohesionSD());
    }



    private void FixedUpdate()
    {
        arbitro();
        applySteering();
    }

    //Realizamos la accion del steeringActual
    public void applySteering()
    {
        if (steeringActual != null)
        {
            //ACCELERATED MOVEMENT
            aceleracion = steeringActual.linear;
            rotacion = steeringActual.angular;

            //capamiento aceleracion
            checkMaxAcelerationReached();

            //si no hay aceleracion paramos -- ya no
            /*if (aceleracion == Vector3.zero)
            {
                velocidad = velocidad.normalized*(System.Math.Max(velocidad.magnitude-movementDeccel*Time.fixedDeltaTime,0)); //--> Parada decelerada, todo por fizik
                //velocidad = Vector3.zero; //Parada en seco
            }
            else
            {
                velocidad += aceleracion*Time.fixedDeltaTime;          //vf = v0 + a*t
            }*/


            velocidad += aceleracion * Time.fixedDeltaTime;          //vf = v0 + a*t
            //capamos velocidad
            checkMaxVelocityReached();
 
            //capamos velocidad angular
            checkMaxRotationReached();



            posicion += velocidad * Time.fixedDeltaTime;   // Fórmulas de Newton 
            orientacion += rotacion * Time.fixedDeltaTime;
            orientacionTo360();
            //ACTUALIZAMOS
            moveTo(posicion);
            //transform.rotation = new Quaternion(); //Quaternion.identity;
            //transform.Rotate(Vector3.up, orientacion);
            transform.eulerAngles = new Vector3(0, orientacion * RadianesAGrados, 0);
            //SI ACABA REINICIAMOS VARIABLES?
            if (selectedBehaviour.finishedLinear)
            {
                velocidad = Vector3.zero;
            }
            if (selectedBehaviour.finishedAngular)
            {
                rotacion = 0;
            }
        }
    }


    //Calculamos para cada SteeringBehavior su Steering y lo añadimos a una lista
    //para despues seleccionar cual se va a aplicar
    /*void LateUpdate()
    {
        arbitro();
    }*/
    

    //Seleccionamos que Steering se va a aplicar recorriendo la lista kinetic
    //actualizando el atributo steeringActual
    private void arbitro()
    {

        //auxiliarmente elegimos el primero de la lista
        if (kinetic.Count > 0)
        {
            steeringActual = kinetic[0].getSteering(this); //se hace antes para hacer el raycast y no hacerlo dos veces
            //CHECK IF WALLAVOIDANCE
            if ((kinetic[0] as WallAvoidance3WhiswersSD).finishedLinear)
            {
                // lo que no es evadir parede
                int i = 1; //not finished
                bool allFinished = true;

                for (i=1; i<kinetic.Count; i++)
                {
                    if (!(kinetic[i].finishedLinear&&kinetic[i].finishedAngular))
                    {
                        selectedBehaviour = kinetic[i];
                        allFinished=false;
                        break;
                    }
                }
                if (allFinished)
                {
                    newTask(new WanderSD(2 * (float)System.Math.PI, 5, 30 * GradosARadianes, 2));
                    selectedBehaviour = kinetic[1];
                }
                else
                {
                    selectedBehaviour = kinetic[i];
                }
                steeringActual = selectedBehaviour.getSteering(this);
            }
            else
            {
                selectedBehaviour = kinetic[0];
            }

            //steering finish
            /*if (steeringActual.angular == 0 && steeringActual.linear == Vector3.zero)
            {
                kinetic.RemoveAt(0);
                steeringActual = null;
            }*/
        }
    }


    internal void checkMaxAcelerationReached()
    {
        if (aceleracion.magnitude>movementAccel)
        {
            aceleracion = aceleracion.normalized * movementAccel;
        }
    }

    internal void checkMaxVelocityReached()
    {
        if (velocidad.magnitude > maxMovementSpeed)
        {
            velocidad = velocidad.normalized * maxMovementSpeed;
        }
    }

    internal void checkMaxRotationReached()
    {
        if (System.Math.Abs(rotacion) > rotationSpeed)
        {
            rotacion = System.Math.Sign(rotacion) * rotationSpeed;
        }
    }

    internal void orientacionTo360()
    {
        if (orientacion < -System.Math.PI)
         {
            orientacion += 2* (float)System.Math.PI;
         }
        else if (orientacion> System.Math.PI)
         {
            orientacion -= 2 * (float)System.Math.PI;
        }
    }

    //GETTERS SETTERS
    internal string nick
    {
        get { return nombre; }
    }
    
    internal void setAction(AgentAction action)
    {
        actionList.Clear();
        actionList.AddFirst(action);
    }

    internal abstract void newTask(SteeringBehaviour st);
    internal abstract void addTask(SteeringBehaviour st);


    internal abstract void disband();


    internal void OnDrawGizmos()
    {
        Vector3 origin = posicion + Vector3.up;
        //Drawing gizmos
        //Whiskers
        Gizmos.color = Color.green;
        /*if (velocidad != Vector3.zero)
        {
            Gizmos.DrawLine(origin, origin + velocidad.normalized * maxMovementSpeed);
            Gizmos.DrawLine(origin, origin + SimulationManager.DirectionToVector(SimulationManager.VectorToDirection(velocidad) + outterAngleVision).normalized * maxMovementSpeed / 2);
            Gizmos.DrawLine(origin, origin + SimulationManager.DirectionToVector(SimulationManager.VectorToDirection(velocidad) - outterAngleVision).normalized * maxMovementSpeed / 2);
        }
        else
        {*/
            Gizmos.DrawLine(origin, origin + SimulationManager.DirectionToVector(orientacion) * velocidad.magnitude*2);
            float leftOri = orientacion - outterAngleVision;
            if (leftOri > System.Math.PI) leftOri -= 2* (float)System.Math.PI;
            else if (leftOri < -System.Math.PI) leftOri += 2*(float)System.Math.PI;
            Gizmos.DrawLine(origin, origin + SimulationManager.DirectionToVector(leftOri) * velocidad.magnitude);
            float rightOri = orientacion + outterAngleVision;
            if (rightOri > System.Math.PI) rightOri -= 2 * (float)System.Math.PI;
            else if (rightOri < -System.Math.PI) rightOri += 2 * (float)System.Math.PI;
            Gizmos.DrawLine(origin, origin + SimulationManager.DirectionToVector(rightOri) * velocidad.magnitude);
        
        //}
        //Area detectors
        Gizmos.color = Color.grey;
        //Gizmos.DrawSphere(posicion, innerDetector);
        Gizmos.color = Color.black;
        //Gizmos.DrawSphere(posicion, outterDetector);
        //Bodi vars
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin+Vector3.up/2, origin + Vector3.up / 2 + velocidad);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin + Vector3.up / 2, origin + Vector3.up / 2 + aceleracion);
    }

}
