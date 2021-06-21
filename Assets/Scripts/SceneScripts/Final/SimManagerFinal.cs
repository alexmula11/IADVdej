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

    [SerializeField]
    protected Transform baseAliada, baseEnemiga;

    static protected Vector2 blocksize;
    static float minX, minY;

    protected float influenceMapTimer = 5f;
    protected float[][] influences;

    static StatsInfo.TIPO_TERRENO[][] terrenos;

    protected new void Start()
    {
        base.Start();
        terrenos = new StatsInfo.TIPO_TERRENO[(int)gridDimensions.x][];
        influences = new float[(int)gridDimensions.x][];
        float widthStep = (mapMaxLimits.x - mapMinLimits.x) / gridDimensions.x;
        float heightStep = (mapMaxLimits.y - mapMinLimits.y) / gridDimensions.y;
        for (int i = 0; i<gridDimensions.x; i++)
        {
            terrenos[i] = new StatsInfo.TIPO_TERRENO[(int)gridDimensions.y];
            influences[i] = new float[(int)gridDimensions.y];
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
                    ui.actualizeUserButtons(allPossibleActions());

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
                                Vector2 posicionOrigen = positionToGrid(person.posicion);
                                AStarSD aEstrella = new AStarSD(terrenos, posicionOrigen, posicionDestino);
                                person.newTaskWOWA(aEstrella);

                            }
                        }
                    }
                }
            }
        }
    }


    private new void FixedUpdate()
    {
        if (influenceMapTimer > 3)
        {
            calculateInfluenceMap();
            ui.actualizeInfluenceMinimap(influences);
            influenceMapTimer = 0;
        }
        else
        {
            influenceMapTimer += Time.fixedDeltaTime;
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
            if (!(person is PersonajeFake))
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
            
        }
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
}
