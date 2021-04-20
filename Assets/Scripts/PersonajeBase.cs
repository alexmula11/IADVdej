using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersonajeBase : Bodi
{
    [SerializeField]
    protected string nombre = "Base Character";
    [SerializeField]
    protected PersonajeNPC fakeObjetive;
    internal PersonajeNPC fake { get { return fakeObjetive; } }



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
    protected SteeringBehaviour selectedBehaviour = null;
    protected Steering steeringActual = null;



    private void Start()
    {
        orientacion = transform.eulerAngles.y * GradosARadianes;
        actionList.AddFirst(new AgentActionStay(orientacion));
        kinetic.Add(new WallAvoidance3WhiswersSD(innerAngleVision,maxMovementSpeed/2,maxMovementSpeed,innerDetector));
        kinetic.Add(new WanderSD(2*(float)System.Math.PI,20));
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
            //ACTUALIZAMOS (SI ACABA REINICIAMOS VARIABLES)              
            transform.position = posicion;
            //transform.rotation = new Quaternion(); //Quaternion.identity;
            //transform.Rotate(Vector3.up, orientacion);
            transform.eulerAngles = new Vector3(0, orientacion * RadianesAGrados, 0);
            if (selectedBehaviour.finished)
            {
                velocidad = Vector3.zero;
                rotacion = 0;
            }

            /*if (selectedBehaviour.steeringType == SteeringBehaviour.MOV_TYPE.ACCELERATED)
            {
                
            }
            else
            {
                velocidad = steeringActual.linear;
                if (velocidad.magnitude > maxMovementSpeed)
                {
                    velocidad = velocidad.normalized * maxMovementSpeed;
                }
                transform.position = posicion;
                transform.eulerAngles = new Vector3(0, orientacion, 0);
            }*/
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
            //CHECK IF WALLAVOIDANCE
            if (kinetic[0].getSteering(this).linear != Vector3.zero)
            {
                selectedBehaviour = kinetic[0];
            }
            else
            {
                selectedBehaviour = kinetic[1];
            }
            steeringActual = selectedBehaviour.getSteering(this);

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
        if (orientacion < -180)
         {
            orientacion += 360;
         }
        else if (orientacion>180)
         {
            orientacion -= 360;
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

    

}
