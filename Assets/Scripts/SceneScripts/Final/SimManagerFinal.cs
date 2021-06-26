using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimManagerFinal : SimulationManager
{
    //min corner is upleft, max corner is rightdown
    //width is Z axis, height X axis (height inverted, negative up)
    [SerializeField]
    protected Vector2 mapMinLimits, mapMaxLimits;
    [SerializeField]
    protected Vector2 gridDimensions;
    protected internal static int maxHeapSize;

    [SerializeField]
    protected GameManager gm;

    static protected Vector2 blocksize;
    static float minX, minY;

    protected float mapsTimer = 2f;
    protected float[][] influences;

    protected Color [][] visibleTerrain;

    public static StatsInfo.TIPO_TERRENO[][] terrenos;


    protected enum MOUSE_ACTION_FINAL
    {
        SELECT = 0,
        MOVE = 1,
        FORM_T = 2,
        FORM_S = 3,
        FORM_R = 4,
        ROUTE_SET = 5,
        ATTACK = 6
    }
    protected new MOUSE_ACTION_FINAL mouseBehav = 0;
    internal override void setMouseBehaviour(int behaviour)
    {
        mouseBehav = (MOUSE_ACTION_FINAL)behaviour;
    }

    protected new void Start()
    {
        PersonajeBase[] personajes = FindObjectsOfType<PersonajeBase>();
        charactersInScene = new List<PersonajeBase>();
        foreach (PersonajeBase person in personajes)
        {
            if (!person.isFake)
            {
                charactersInScene.Add(person);
                gm.AddMatraca(person);
            }
        }

        terrenos = new StatsInfo.TIPO_TERRENO[(int)gridDimensions.x][];
        influences = new float[(int)gridDimensions.x][];
        visibleTerrain = new Color[(int)gridDimensions.x][];

        float widthStep = (mapMaxLimits.x - mapMinLimits.x) / gridDimensions.x;
        float heightStep = (mapMaxLimits.y - mapMinLimits.y) / gridDimensions.y;
        for (int i = 0; i<gridDimensions.x; i++)
        {
            terrenos[i] = new StatsInfo.TIPO_TERRENO[(int)gridDimensions.y];            //Inic terrenos, influences y terrenos visibles
            influences[i] = new float[(int)gridDimensions.y];
            visibleTerrain[i] = new Color[(int)gridDimensions.y];

            for (int j = 0; j < gridDimensions.y; j++)
            {
                Vector3 originPoint = new Vector3(-(mapMinLimits.y + j * heightStep),0.5f,mapMinLimits.x+i*widthStep);
                RaycastHit hit;
                if (Physics.Raycast(originPoint, Vector3.down, out hit, 1f, 1 << 10))
                {
                    terrenos[i][j] = hit.collider.GetComponent<GroundTypeDelegate>().tipoTerreno;
                }

            }
        }
        blocksize = new Vector2(widthStep, heightStep);
        minX = mapMinLimits.x;
        minY = mapMinLimits.y;
        maxHeapSize = ((int)gridDimensions.x) * ((int)gridDimensions.y);
        gm.initIAs();
    }


    protected new void Update()
    {
        if (!mouseOverUI)
        {
            if (mouseBehav == MOUSE_ACTION_FINAL.SELECT)
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
                    ui.actualizeUserButtons(allPossibleActions());

                }
            }
            else if (mouseBehav == MOUSE_ACTION_FINAL.MOVE)
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
                                Vector2 posicionOrigen = positionToGrid(person.posicion);

                                ActionGo moverse = new ActionGo(person, posicionDestino,null);
                                person.accion = moverse;
                                person.accion.doit();
                            }
                        }
                        else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 13))
                        {

                        }
                    }
                }
            }
            else if (mouseBehav == MOUSE_ACTION_FINAL.FORM_T || mouseBehav == MOUSE_ACTION_FINAL.FORM_S || mouseBehav == MOUSE_ACTION_FINAL.FORM_R)
            {
                if (selectedUnits.Count > 0)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 8))
                        {

                            // PARA FORMACIONES
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

                            switch (mouseBehav)
                            {
                                case MOUSE_ACTION_FINAL.FORM_T:
                                    formacion = new FormacionTriangulo(lider);
                                    break;
                                case MOUSE_ACTION_FINAL.FORM_S:
                                    formacion = new FormacionCuadrado(lider);
                                    break;
                                case MOUSE_ACTION_FINAL.FORM_R:
                                    formacion = new FormacionPorRoles(lider);
                                    break;
                            }
                            foreach (PersonajeBase person in selectedUnits)
                            {
                                person.currentFormacion = formacion;
                                if (person != lider)
                                {
                                    formacion.addMiembro(person);
                                }
                            }
                            formacion.formacionASusPuestosAccion();
                            //formacion.formacionASusPuestos();
                            formaciones.Add(formacion);
                        }
                    }
                }
            }
            else if (mouseBehav == MOUSE_ACTION_FINAL.ROUTE_SET)
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
                            /*foreach (PersonajeBase person in selectedUnits)
                            {
                                if (person.currentFormacion != null)
                                {
                                    formationManager.removeFormacion(person.currentFormacion);
                                    person.currentFormacion.disband();
                                }
                            }*/
                        }
                        else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10000f, 1 << 10))
                        {
                            if (pathToSet.Count == 0)
                            {
                                foreach (PersonajeBase unit in selectedUnits)
                                {
                                    foreach (Transform routeElem in unit.routeMarks)
                                    {
                                        Destroy(routeElem.gameObject);
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


    private new void FixedUpdate()
    {
        if (mapsTimer > 1)
        {
            calculateInfluenceMap();
            ui.actualizeInfluenceMinimap(influences);
            calculateVisionMap();
            ui.actualizeVisionMinimap(visibleTerrain);
            mapsTimer = 0;
        }
        else
        {
            mapsTimer += Time.fixedDeltaTime;
        }
    }

    private void calculateInfluenceMap()
    {
        for (int i = 0; i < gridDimensions.x; i++)
        {
            for (int j = 0; j < gridDimensions.y; j++)
            {
                influences[i][j] = 0f;
            }
        }


        foreach (PersonajeBase person in charactersInScene)
        {
            Vector2 origenInfluencia = positionToGrid(person.posicion);
            float distanciaInfluencia = StatsInfo.distanciaInfluenciaUnidades[(int)person.tipo];
            float potenciaInfluencia = StatsInfo.potenciaInfluenciaUnidades[(int)person.tipo];
            for (int i = (int)System.Math.Max((origenInfluencia.x - distanciaInfluencia), 0); i < (int)System.Math.Min((origenInfluencia.x + distanciaInfluencia), gridDimensions.x - 1); i++)
            {
                for (int j = (int)System.Math.Max((origenInfluencia.y - distanciaInfluencia), 0); j < (int)System.Math.Min((origenInfluencia.y + distanciaInfluencia), gridDimensions.y - 1); j++)
                {
                    float distanciaActual = (new Vector2(i, j) - origenInfluencia).magnitude;
                    float distanciaRelativa = System.Math.Min(1, distanciaActual / distanciaInfluencia);
                    float influenciaActual = potenciaInfluencia - potenciaInfluencia * distanciaRelativa;
                    if (terrenos[i][j] != StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
                    {
                        if (person is PersonajePlayer)
                        {
                            influences[i][j] = influences[i][j] + influenciaActual;
                        }
                        else if (person)
                        {
                            influences[i][j] = influences[i][j] - influenciaActual;
                        }
                    }
                }
            }
        }
        //BASE ALIADA
        Vector2 origenInfluenciaBase = positionToGrid(gm.allyBase.position);
        for (int i = (int)System.Math.Max((origenInfluenciaBase.x - StatsInfo.baseDistanciaInfluencia), 0); i < (int)System.Math.Min((origenInfluenciaBase.x + StatsInfo.baseDistanciaInfluencia), gridDimensions.x - 1); i++)
        {
            for (int j = (int)System.Math.Max((origenInfluenciaBase.y - StatsInfo.baseDistanciaInfluencia), 0); j < (int)System.Math.Min((origenInfluenciaBase.y + StatsInfo.baseDistanciaInfluencia), gridDimensions.y - 1); j++)
            {
                float distanciaActual = (new Vector2(i, j) - origenInfluenciaBase).magnitude;
                float distanciaRelativa = System.Math.Min(1, distanciaActual / StatsInfo.baseDistanciaInfluencia);
                float influenciaActual = StatsInfo.basePotenciaInfluencia - StatsInfo.basePotenciaInfluencia * distanciaRelativa;
                if (terrenos[i][j] != StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
                {
                    influences[i][j] = influences[i][j] + influenciaActual;
                }
            }
        }

        //BASE ENEMIGA
        origenInfluenciaBase = positionToGrid(gm.enemyBase.position);
        for (int i = (int)System.Math.Max((origenInfluenciaBase.x - StatsInfo.baseDistanciaInfluencia), 0); i < (int)System.Math.Min((origenInfluenciaBase.x + StatsInfo.baseDistanciaInfluencia), gridDimensions.x - 1); i++)
        {
            for (int j = (int)System.Math.Max((origenInfluenciaBase.y - StatsInfo.baseDistanciaInfluencia), 0); j < (int)System.Math.Min((origenInfluenciaBase.y + StatsInfo.baseDistanciaInfluencia), gridDimensions.y - 1); j++)
            {
                float distanciaActual = (new Vector2(i, j) - origenInfluenciaBase).magnitude;
                float distanciaRelativa = System.Math.Min(1, distanciaActual / StatsInfo.baseDistanciaInfluencia);
                float influenciaActual = StatsInfo.basePotenciaInfluencia - StatsInfo.basePotenciaInfluencia * distanciaRelativa;
                if (terrenos[i][j] != StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
                {
                    influences[i][j] = influences[i][j] - influenciaActual;
                }
            }
        }
    }

    
    private void calculateVisionMap()
    {
        //mirar con distancia de vision de unidades   
        //colorear con el color del terreno la porcion visible
        //TODO poner punto de color en el pesonaje que aporta vision
        for (int i = 0; i < gridDimensions.x; i++)                  //Reinic la matriz
        {
            for (int j = 0; j < gridDimensions.y; j++)
            {
                visibleTerrain[i][j] = StatsInfo.coloresTerrenos[0];
            }
        }


        foreach (PersonajeBase person in charactersInScene)
        {

            Vector2 origenVision = positionToGrid(person.posicion);
            float distanciaVision = StatsInfo.distanciaVisionUnidades[(int)person.tipo];

            for (int i = (int)System.Math.Max((origenVision.x - distanciaVision), 0); i < (int)System.Math.Min((origenVision.x + distanciaVision), gridDimensions.x - 1); i++)
            {
                for (int j = (int)System.Math.Max((origenVision.y - distanciaVision), 0); j < (int)System.Math.Min((origenVision.y + distanciaVision), gridDimensions.y - 1); j++)
                {
                    Vector2 puntoActual = new Vector2(i, j);
                    if ((puntoActual - origenVision).magnitude <= distanciaVision)
                    {
                        visibleTerrain[i][j] = StatsInfo.coloresTerrenos[(int)terrenos[i][j]];
                    }
                }
            }
        }
        //BASE ALIADA
        Vector2 origenInfluenciaBase = positionToGrid(gm.allyBase.position);
        for (int i = (int)System.Math.Max((origenInfluenciaBase.x - StatsInfo.baseDistanciaInfluencia), 0); i < (int)System.Math.Min((origenInfluenciaBase.x + StatsInfo.baseDistanciaInfluencia), gridDimensions.x - 1); i++)
        {
            for (int j = (int)System.Math.Max((origenInfluenciaBase.y - StatsInfo.baseDistanciaInfluencia), 0); j < (int)System.Math.Min((origenInfluenciaBase.y + StatsInfo.baseDistanciaInfluencia), gridDimensions.y - 1); j++)
            {
                Vector2 puntoActual = new Vector2(i, j);
                if ((puntoActual - origenInfluenciaBase).magnitude <= StatsInfo.baseDistanciaInfluencia)
                {
                    visibleTerrain[i][j] = StatsInfo.coloresTerrenos[(int)terrenos[i][j]];
                }
            }
        }
        //BASE ENEMIGA
        origenInfluenciaBase = positionToGrid(gm.enemyBase.position);
        for (int i = (int)System.Math.Max((origenInfluenciaBase.x - StatsInfo.baseDistanciaInfluencia), 0); i < (int)System.Math.Min((origenInfluenciaBase.x + StatsInfo.baseDistanciaInfluencia), gridDimensions.x - 1); i++)
        {
            for (int j = (int)System.Math.Max((origenInfluenciaBase.y - StatsInfo.baseDistanciaInfluencia), 0); j < (int)System.Math.Min((origenInfluenciaBase.y + StatsInfo.baseDistanciaInfluencia), gridDimensions.y - 1); j++)
            {
                Vector2 puntoActual = new Vector2(i, j);
                if ((puntoActual - origenInfluenciaBase).magnitude <= StatsInfo.baseDistanciaInfluencia)
                {
                    visibleTerrain[i][j] = StatsInfo.coloresTerrenos[(int)terrenos[i][j]];
                }
            }
        }

        foreach (PersonajeBase person in charactersInScene)
        {
            Vector2 origenVision = positionToGrid(person.posicion);
            if (person is PersonajePlayer)
                visibleTerrain[(int)origenVision.x][(int)origenVision.y] = Color.blue;
            else
                visibleTerrain[(int)origenVision.x][(int)origenVision.y] = Color.red;
        }
        visibleTerrain[(int)origenInfluenciaBase.x][(int)origenInfluenciaBase.y] = Color.blue;
        visibleTerrain[(int)origenInfluenciaBase.x][(int)origenInfluenciaBase.y] = Color.red;
    }

    internal static Vector2 positionToGrid(Vector3 position)
    {
        //int coordX = (int)(System.Math.Floor(System.Math.Round(position.z) / blocksize.x) + System.Math.Floor((float)terrenos.Length / 2));
        int coordX = (int)System.Math.Floor((position.z - minX)/blocksize.x);
        //int coordY = (int)(System.Math.Floor(System.Math.Round(position.x) / blocksize.y) + System.Math.Floor((float)terrenos[0].Length / 2));
        int coordY = (int)System.Math.Floor((position.x - minY) / blocksize.y);
        return new Vector2(coordX, coordY);
    }

    internal static Vector3 gridToPosition(Vector2 gridPos)
    {
        //Math ceiling para nuestro mapa concreto
        float coordX = (gridPos.y - terrenos[0].Length / 2f) * blocksize.y + (float)System.Math.Ceiling(blocksize.y/2);
        float coordY = (gridPos.x - terrenos.Length / 2f) * blocksize.x + (float)System.Math.Ceiling(blocksize.x / 2);
        return new Vector3(coordX, 0, coordY);
    }

    private HashSet<StatsInfo.ACCION> allPossibleActions()
    {
        HashSet<StatsInfo.ACCION> lista = new HashSet<StatsInfo.ACCION>();
        foreach (PersonajeBase person in selectedUnits)
        {
            foreach (StatsInfo.ACCION action in person.posiblesAcciones)
            {
                lista.Add(action);
            }
        }
        return lista;
    }


    internal static List<Vector2> aStarPath(Vector2 origen, Vector2 end, StatsInfo.TIPO_PERSONAJE tipo)
    {
        NodoGrafoAStar nodoOrigen = new NodoGrafoAStar(origen, (end - origen).magnitude, 0f, null);
        LinkedList<Vector2> recorrido = new LinkedList<Vector2>();
        float estimatedCost = (end - origen).magnitude;

        NodoGrafoAStar[][] nodos = new NodoGrafoAStar[165][];
        for (int i=0; i<nodos.Length; i++)
        {
            nodos[i] = new NodoGrafoAStar[65];
        }

        LinkedList<NodoGrafoAStar> closedPositions = new LinkedList<NodoGrafoAStar>();

        Heap<NodoGrafoAStar> openPositions = new Heap<NodoGrafoAStar>(maxHeapSize);
        openPositions.Add(nodoOrigen);

        NodoGrafoAStar nodoActual = null;

        while (openPositions.Count > 0)
        {
            nodoActual = openPositions.RemoveFirst();
            closedPositions.AddFirst(nodoActual);

            if(nodoActual.posicionGrid == end)
            {
                break;
            }

            LinkedList<NodoGrafoAStar> adyacentes = calcularAdyacentes(nodoActual, end,nodos,tipo);
            //LinkedList<NodoGrafoAStar> adyacentesFiltrados 
            foreach (NodoGrafoAStar nodito in adyacentes)
            {
                //if closed.contains(neighbour)
                bool estaEnListaClosed = false;
                foreach (NodoGrafoAStar noditoClosed in closedPositions)
                {
                    //Si el nodo ya está en la lista closed, no se considera
                    if (noditoClosed.posicionGrid == nodito.posicionGrid)
                    {
                        estaEnListaClosed = true;
                        break;
                    }
                }
                if (estaEnListaClosed)
                {
                    continue;
                }
                //calculamos distancia al siguiente nodo desde el que estamos
                float inversaVelocidad = 1 / StatsInfo.velocidadUnidadPorTerreno[(int)terrenos[(int)nodito.posicionGrid.x][(int)nodito.posicionGrid.y]][(int)tipo];
                //float newG = nodoActual.costFromOrigin + (nodoActual.posicionGrid - nodito.posicionGrid).magnitude * inversaVelocidad;
                float newG =  (int)(nodoActual.costFromOrigin + Mathf.RoundToInt((nodoActual.posicionGrid - nodito.posicionGrid).magnitude * inversaVelocidad));

                if (newG < nodito.costFromOrigin || (nodos[(int)nodito.posicionGrid.x][(int)nodito.posicionGrid.y]!=null && !openPositions.Contains(nodito))) {
					nodito.costFromOrigin = newG;
					//nodito.estimatedCost = (end-nodito.posicionGrid).magnitude;
                    nodito.estimatedCost = getDistance(nodito,end);
					nodito.padre = nodoActual;

					if (nodos[(int)nodito.posicionGrid.x][(int)nodito.posicionGrid.y] != null && !openPositions.Contains(nodito))
						openPositions.Add(nodito);
					else {
						openPositions.UpdateItem(nodito);
					}
				}
            }
        }

        //Calculamos el camino a seguir en base a los padres del nodo destino
        NodoGrafoAStar aux = nodoActual;
        while (aux.padre != null)
        {
            recorrido.AddFirst(aux.posicionGrid);
            aux = aux.padre;
        }
        
        return new List<Vector2>(recorrido);
    }

    internal static List<Vector3> aStarPathV3(Vector2 origen, Vector2 end, StatsInfo.TIPO_PERSONAJE tipo)
    {
        List<Vector2> camino = aStarPath(origen, end, tipo);
        List<Vector3> caminoV3 = new List<Vector3>();
        foreach (Vector2 pos in camino)
        {
            caminoV3.Add(gridToPosition(pos));
        }
        return caminoV3;
    }

    private static LinkedList<NodoGrafoAStar> calcularAdyacentes(NodoGrafoAStar actual,Vector2 destino, NodoGrafoAStar[][] nodosUsados, StatsInfo.TIPO_PERSONAJE type)
    {
        LinkedList<NodoGrafoAStar> listanodos = new LinkedList<NodoGrafoAStar>();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                //8 vecinos
                if (i != 0 || j != 0)
                //4 vecinos
                //if (System.Math.Abs(i) != System.Math.Abs(j))
                {
                    Vector2 newPosi = new Vector2(actual.posicionGrid.x + i, actual.posicionGrid.y + j);
                    if (terrenos[(int)newPosi.x][(int)newPosi.y] != StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
                    {
                        NodoGrafoAStar nodoActual = nodosUsados[(int)newPosi.x][(int)newPosi.y];
                        if (nodoActual!=null)
                        {
                            listanodos.AddLast(nodoActual);
                        }
                        else
                        {
                            float inversaVelocidad = 1 / StatsInfo.velocidadUnidadPorTerreno[(int)terrenos[(int)newPosi.x][(int)newPosi.y]][(int)type];
                            float newG = (int)(actual.costFromOrigin + Mathf.RoundToInt((actual.posicionGrid - newPosi).magnitude * inversaVelocidad));
                            NodoGrafoAStar nuevoNodo = new NodoGrafoAStar(newPosi, getDistance(newPosi, destino), newG, actual);
                            listanodos.AddLast(nuevoNodo);
                            nodosUsados[(int)newPosi.x][(int)newPosi.y] = nuevoNodo;
                        }

                    }

                }
            }
        }
        return listanodos;
    }

    private static int getDistance(NodoGrafoAStar origin, NodoGrafoAStar destiny)
    {
        int dstX = (int)Mathf.Abs(origin.posicionGrid.x - destiny.posicionGrid.x);
		int dstY = (int)Mathf.Abs(origin.posicionGrid.y - destiny.posicionGrid.y);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);           //el 14 y el 10 depende del tamaño de nuestro grid
		return 14*dstX + 10 * (dstY-dstX);
    }

    private static int getDistance(NodoGrafoAStar origin, Vector2 destiny)
    {
        int dstX = (int)Mathf.Abs(origin.posicionGrid.x - destiny.x);
		int dstY = (int)Mathf.Abs(origin.posicionGrid.y - destiny.y);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);           //el 14 y el 10 depende del tamaño de nuestro grid
		return 14*dstX + 10 * (dstY-dstX);
    }

    private static int getDistance(Vector2 origin, Vector2 destiny)
    {
        int dstX = (int)Mathf.Abs(origin.x - destiny.x);
        int dstY = (int)Mathf.Abs(origin.y - destiny.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);           //el 14 y el 10 depende del tamaño de nuestro grid
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
