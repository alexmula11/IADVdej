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
    internal float innerAngleVision=Mathf.PI/6 , outterAngleVision=Mathf.PI/2/(2/3); //30grad -- 60grad

    
    
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
        orientacion = transform.eulerAngles.y;
        actionList.AddFirst(new AgentActionStay(orientacion));
        kinetic.Add(new WanderSteering(360,20));
    }



    private void FixedUpdate()
    {
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

            //si no hay aceleracion paramos
            if (aceleracion == Vector3.zero)
            {
                //velocidad = velocidad.normalized*(velocidad.magnitude-movementAccel); --> Parada decelerada NO
                velocidad = Vector3.zero; //Parada en seco
            }
            //capamiento aceleracion
            checkMaxAcelerationReached();
  
            velocidad = velocidad + aceleracion * Time.fixedDeltaTime;          //vf = v0 + a*t

            //capamos velocidad
            checkMaxVelocityReached();
 
            //capamos velocidad angular
            checkMaxRotationReached();
 


            //ACTUALIZAMOS
            posicion += velocidad * Time.fixedDeltaTime;                        // Fórmulas de Newton 
            transform.position = posicion;
            orientacion += rotacion  * Time.fixedDeltaTime;
            
            orientacionTo360();

            //transform.rotation = new Quaternion(); //Quaternion.identity;
            //transform.Rotate(Vector3.up, orientacion);
            transform.eulerAngles = new Vector3(0, orientacion, 0);
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
    void LateUpdate()
    {
        arbitro();
    }
    

    //Seleccionamos que Steering se va a aplicar recorriendo la lista kinetic
    //actualizando el atributo steeringActual
    private void arbitro()
    {

        //auxiliarmente elegimos el primero de la lista
        if (kinetic.Count > 0)
        {
            selectedBehaviour = kinetic[0];
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
