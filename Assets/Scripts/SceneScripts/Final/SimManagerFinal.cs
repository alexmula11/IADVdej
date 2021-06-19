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

    static protected Vector2 blocksize;
    static float minX, minY;
    

    static StatsInfo.TIPO_TERRENO[][] terrenos; 

    protected new void Start()
    {
        terrenos = new StatsInfo.TIPO_TERRENO[(int)gridDimensions.x][];
        float widthStep = (mapMaxLimits.x - mapMinLimits.x) / gridDimensions.x;
        float heightStep = (mapMaxLimits.y - mapMinLimits.y) / gridDimensions.y;
        for (int i = 0; i<gridDimensions.x; i++)
        {
            terrenos[i] = new StatsInfo.TIPO_TERRENO[(int)gridDimensions.y];
            for (int j = 0; j < gridDimensions.y; j++)
            {
                Debug.Log("Cogiendo coordenada de grid: "+i+"x"+j);
                Vector3 originPoint = new Vector3(-(mapMinLimits.y + j * heightStep),0.5f,mapMinLimits.x+i*widthStep);
                RaycastHit hit;
                if (Physics.Raycast(originPoint, Vector3.down, out hit, 1f, 1 << 10))
                {
                    terrenos[i][j] = hit.collider.GetComponent<GroundTypeDelegate>().tipoTerreno;
                    Debug.Log("Asignando terreno: " + terrenos[i][j]);
                }

            }
        }
        blocksize = new Vector2(widthStep, heightStep);
        minX = mapMinLimits.x;
        minY = mapMinLimits.y;
    }


    protected new void Update()
    {
        Vector2 final = new Vector2(gridDimensions.x-1, gridDimensions.y-1);
        Vector3 centroFinal = gridToPosition(final);
        Vector3 aunFinal = centroFinal - Vector3.forward * 2;
        Vector3 yaNoFinal = centroFinal - Vector3.forward * 3;
        final = new Vector2(0, 0);
        Vector3 otroFinal = gridToPosition(final);
        Vector3 aunOtroFinal = otroFinal - Vector3.back * 2;
        Vector3 yaNoOtroFinal = otroFinal - Vector3.back * 3;
        bool hola = true;
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
}
