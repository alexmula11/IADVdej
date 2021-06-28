using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class PersonajeBase : Bodi
{
    private bool escenaFinal = false;
    private bool recalcularAccion = false;

    [SerializeField]
    protected string nombre = "Base Character";
    [SerializeField]
    protected PersonajeNPC fakeMovementObjetive, fakeAvoidObjetive, fakeAlignObjetive;
    [SerializeField]
    protected GameObject[] gizmosGOs;
    [SerializeField]
    protected Transform routeMarkers;
    [SerializeField]
    protected MeshRenderer bodyMesh, headMesh;

    [SerializeField]
    protected bool fake;

    internal bool isFake { get { return fake; } }

    internal PersonajeNPC fakeMovement { get { return fakeMovementObjetive; } }
    internal PersonajeNPC fakeAvoid { get { return fakeAvoidObjetive; } }
    internal PersonajeNPC fakeAlign { get { return fakeAlignObjetive; } }
    internal Transform routeMarks { get { return routeMarkers; } }

    protected Formacion formacion = null;
    internal Formacion currentFormacion { get{ return formacion; } set { formacion = value; } }

    internal List<PersonajeBase> group = new List<PersonajeBase>();

    [SerializeField]
    protected internal StatsInfo.TIPO_PERSONAJE tipo = StatsInfo.TIPO_PERSONAJE.INFANTERIA;

    //SENSORS
    [SerializeField]
    internal float innerDetector=2f, outterDetector=10f;
    internal float innerAngleVision=5*GradosARadianes, outterAngleVision=30*GradosARadianes; 

    
    
    //TURNING MANAGE
    internal float stayLookingDirection;
    //MOVEMENT MANAGE
    internal float currentMovementSpeed { get { return velocidad.magnitude; } }

    protected internal Accion currentAction;
    protected internal Accion accion { get { return currentAction; } set { currentAction = value; } }

    protected List<SteeringBehaviour> kinetic = new List<SteeringBehaviour>();
    protected internal SteeringBehaviour selectedBehaviour = null;
    protected internal SteeringBehaviour antiDorifto = null;
    protected internal Steering steeringActual = null;

    protected internal SteeringBehaviour defaultSteering = new WanderSD(2 * (float)System.Math.PI, 5, 30 * GradosARadianes, 2);
    protected internal StatsInfo.ACCION[] posiblesAcciones;
    //TERRAIN BELOW
    protected StatsInfo.TIPO_TERRENO terrenoQuePiso = 0;


    /*COMBAT ATRIBUTES*/
    [SerializeField]
    protected HPMarker hPMarker;
    protected internal float health;


    public bool isAlive()
    {
        return (this.health > 0);
    }

    public void actualizeHealth(float healthInput)  //esto pide a gritos un mutex
    {
        health += healthInput;
        if (health <= 0)
        {
            health = 0;
            GameManager.addMuerto(this);
            headMesh.enabled = false;
            bodyMesh.enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            hPMarker.gameObject.SetActive(false);
            disbandAccion();
        }
        else if (health > StatsInfo.healthPerClass[(int)tipo]) health = StatsInfo.healthPerClass[(int)tipo];
        float percent = health/StatsInfo.healthPerClass[(int)tipo];
        hPMarker.changeActualHP(percent);
    }

    public bool isFullHealth()
    {
        return this.health == StatsInfo.healthPerClass[(int)tipo];
    }

    public bool isInCombat()
    {
        return (currentAction.nombre != "ATACAR");
    }

    public bool betterToRun()
    {
        return this.health < StatsInfo.healthPerClass[(int)tipo]*0.3;
    }

    private void Start()
    {
        escenaFinal = SceneManager.GetActiveScene().name == "GeneralStrategy";
        orientacion = transform.eulerAngles.y * GradosARadianes;
        if (!fake) applyTipo(tipo);
        
        
        //VARIAR COMO EMPIEZA EL PERSONAJE EN FUNCIÓN DE LA ESCENA

        //actionList.AddFirst(new AgentActionStay(orientacion));
        //newTask(new WanderSD(2 * (float)System.Math.PI, 5, 30 * GradosARadianes, 2));
        //newTask(new CohesionSD());
    }


    protected internal void applyTipo(StatsInfo.TIPO_PERSONAJE tipo)
    {
        this.tipo = tipo;
        this.health = StatsInfo.healthPerClass[(int)tipo];
        
        /*switch (tipo)
        {
            case UnitsInfo.TIPO_PERSONAJE.INFANTERIA:
                bodyMesh.material.color = UnitsInfo.colorInfanteria;
                maxMovementSpeed = UnitsInfo.velocidadUnidadPorTerreno[0][(int)tipo];
                break;
            case UnitsInfo.TIPO_PERSONAJE.ARQUERO:
                bodyMesh.material.color = UnitsInfo.colorArquero;
                maxMovementSpeed = UnitsInfo.velocidadArquero;
                break;
            case UnitsInfo.TIPO_PERSONAJE.PESADA:
                bodyMesh.material.color = UnitsInfo.colorPesada;
                maxMovementSpeed = UnitsInfo.velocidadPesada;
                break;
        }*/
        bodyMesh.material.color = StatsInfo.coloresUnidades[(int) tipo];
        maxMovementSpeed = StatsInfo.velocidadUnidades[(int)tipo];
        posiblesAcciones = StatsInfo.accionesDeUnidades[(int)tipo];
        if (this is PersonajePlayer)
        {
            //headMesh.material.color = StatsInfo.colorPlayerTeam;
            hPMarker.setHpColor(StatsInfo.colorPlayerTeam);
        }
        else
        {
            //headMesh.material.color = StatsInfo.colorIATeam;
            hPMarker.setHpColor(StatsInfo.colorIATeam);
        }
    }


    protected void FixedUpdate()
    {
        if (!fake)
        {
            checkTerrainBelow();
            if (escenaFinal)
            {
                arbitroEscenaFinal();
                hPMarker.setPosition(posicion);
            }
            else
                arbitro();
            applySteering();
        }
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
            if (selectedBehaviour==null || selectedBehaviour.finishedLinear)
            {
                velocidad = Vector3.zero;
            }
            if (selectedBehaviour == null || selectedBehaviour.finishedAngular)
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

            //steeringActual = kinetic[0].getSteering(this); //se hace antes para hacer el raycast y no hacerlo dos veces
            //CHECK IF WALLAVOIDANCE
            int i=0;
            if ((kinetic[0] as WallAvoidance3WhiswersSD) != null)
            {
                steeringActual = kinetic[0].getSteering(this);
                if ((kinetic[0] as WallAvoidance3WhiswersSD).finishedLinear)
                {
                    i = 1;
                    // lo que no es evadir parede
                    bool allFinished = true;

                    for (i = 1; i < kinetic.Count; i++)
                    {
                        if (!(kinetic[i].finishedLinear && kinetic[i].finishedAngular))
                        {
                            selectedBehaviour = kinetic[i];
                            allFinished = false;
                            break;
                        }
                    }
                    if (allFinished)
                    {
                        newTask(defaultSteering);
                        selectedBehaviour = kinetic[1];
                    }
                    else
                    {
                        selectedBehaviour = kinetic[i];
                    }
                    if (allFinished)
                    {
                        i--;
                    }
                    selectedBehaviour = kinetic[i];
                    steeringActual = selectedBehaviour.getSteering(this);
                }
                else
                {
                    selectedBehaviour = kinetic[0];
                }
            }
            else
            {
                bool allFinished = true;
                for (i = i; i < kinetic.Count; i++)
                {
                    if (!(kinetic[i].finishedLinear && kinetic[i].finishedAngular))
                    {
                        selectedBehaviour = kinetic[i];
                        allFinished = false;
                        break;
                    }
                }
                if (allFinished)
                {
                    newTask(defaultSteering);
                    selectedBehaviour = kinetic[1];
                }
                else
                {
                    selectedBehaviour = kinetic[i];
                }
                selectedBehaviour = kinetic[i];
                steeringActual = selectedBehaviour.getSteering(this);
            }
                //CUANDO NO HAY WALL AVOIDANCE
                //selectedBehaviour = kinetic[0];

                //steering finish
                /*if (steeringActual.angular == 0 && steeringActual.linear == Vector3.zero)
                {
                    kinetic.RemoveAt(0);
                    steeringActual = null;
                }*/
        }
    }

    private void arbitroEscenaFinal()
    {
        if (isAlive())
        {
            if (currentAction != null && currentAction is AccionCompuesta)       //siguiente accion de la lista
            {
                ((AccionCompuesta)currentAction).actualizeAction(); //aceder a currentaction 
            }
            if (currentAction != null && currentAction.isDone())
            {
                kinetic.Clear();
                currentAction = null;
                selectedBehaviour = null;
                steeringActual = new Steering();
            }
            else
            {
                //ESTO ESTA MAL DE ALGUNA FORMA

                //auxiliarmente elegimos el primero de la lista
                if (kinetic.Count > 0)
                {
                    kinetic[0].getSteering(this);
                    if ((kinetic[0] as WallAvoidance3WhiswersGridSD).finishedLinear)
                    {
                        if (recalcularAccion)
                        {
                            recalcularAccion = false;
                            if(currentAction != null)currentAction.doit();
                        }
                        selectedBehaviour = kinetic[1];
                        actuadorHumanoNoDriftin();
                    }
                    else
                    {
                        recalcularAccion = true;
                        selectedBehaviour = kinetic[0];
                    }                     
                    steeringActual = selectedBehaviour.getSteering(this);
                }
            }
        }   
    }

    private void actuadorHumanoNoDriftin()
    {
        if(selectedBehaviour != antiDorifto)
        {
            this.velocidad = Vector3.zero;
            antiDorifto = selectedBehaviour;
        }
    }

    private void checkTerrainBelow()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position+Vector3.up,Vector3.down, out hit, 2, 1 << 10))
        {
            
            terrenoQuePiso = hit.collider.GetComponent<GroundTypeDelegate>().tipoTerreno;
            maxMovementSpeed = StatsInfo.velocidadUnidades[(int)tipo] * StatsInfo.velocidadUnidadPorTerreno[(int)terrenoQuePiso][(int)tipo];
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
    

    internal abstract void newTask(SteeringBehaviour st);
    internal abstract void addTask(SteeringBehaviour st);
    internal abstract void newTaskWOWA(SteeringBehaviour st);
    internal abstract void newTaskLowWA(SteeringBehaviour st);
    internal abstract void newTaskGrid(SteeringBehaviour st);


    internal abstract void disband();

    internal abstract void disbandAccion();


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
            //Gizmos.DrawLine(origin, origin + SimulationManager.DirectionToVector(orientacion) * velocidad.magnitude*2.5f);
            Gizmos.DrawLine(origin, origin + SimulationManager.DirectionToVector(orientacion) * maxMovementSpeed*2f);
            float leftOri = orientacion - outterAngleVision;
            if (leftOri > System.Math.PI) leftOri -= 2* (float)System.Math.PI;
            else if (leftOri < -System.Math.PI) leftOri += 2*(float)System.Math.PI;
            //Gizmos.DrawLine(origin, origin + SimulationManager.DirectionToVector(leftOri) * velocidad.magnitude * 1.5f);
            Gizmos.DrawLine(origin, origin + SimulationManager.DirectionToVector(leftOri) * maxMovementSpeed*1.5f);
            float rightOri = orientacion + outterAngleVision;
            if (rightOri > System.Math.PI) rightOri -= 2 * (float)System.Math.PI;
            else if (rightOri < -System.Math.PI) rightOri += 2 * (float)System.Math.PI;
            //Gizmos.DrawLine(origin, origin + SimulationManager.DirectionToVector(rightOri) * velocidad.magnitude * 1.5f);
            Gizmos.DrawLine(origin, origin + SimulationManager.DirectionToVector(rightOri) * maxMovementSpeed * 1.5f);
        
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


    protected internal void revive(Vector3 spawnPoint)
    {
        headMesh.enabled = true;
        bodyMesh.enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        hPMarker.gameObject.SetActive(true);
        actualizeHealth(StatsInfo.healthPerClass[(int)tipo]);

        moveTo(spawnPoint);
        posicion = spawnPoint;
    }

}
