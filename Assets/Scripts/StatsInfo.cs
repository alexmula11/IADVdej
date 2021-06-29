using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsInfo
{

    private const int MAX_UNIT_TYPE = 4;


    public enum TIPO_PERSONAJE
    {
        INFANTERIA = 0,
        ARQUERO = 1,
        PESADA = 2,
        MAJITO = 3
    }

    public enum TIPO_TERRENO
    {
        INFRANQUEABLE,
        CEMENTO,
        LLANURA,
        BOSQUE,
        ARENA,
        MADERA,
    }

    public enum ACCION
    {
        MOVE_ATTACK_OR_FOLLOW,
        FORMATION,
        ROUTE
    }


    public static float[] velocidadUnidades =
    {
        1.5f, 2, 1, 1
    };
    /*
     * primer acceso tipo de terreno, segundo acceso tipo de unidad
     * 0 infranqueable (escenas específicas)
     * 1 cemento
     * 2 llanura
     * 3 bosque
     * 4 arena
     * 5 madera (puentes)
     */

     /*public static float[][] velocidadUnidadPorTerreno =
    {
        new float[MAX_UNIT_TYPE] { 1, 1, 1, 1},
        new float[MAX_UNIT_TYPE] { 1.5f, 1.5f, 2, 2},
        new float[MAX_UNIT_TYPE] { 1, 1, 1, 1},
        new float[MAX_UNIT_TYPE] { 0.66f, 0.75f, 0.5f, 0.5f},
        new float[MAX_UNIT_TYPE] { 0.66f, 0.66f, 0.5f, 0.5f},
        new float[MAX_UNIT_TYPE] { 1.5f, 1.5f, 1.5f, 1.5f},
    };*/

    public static Vector2 [] puente_superior =
    {
        new Vector2(75,20),
        new Vector2(75,24),
        new Vector2(89,20),
        new Vector2(89,24)
    };
    public static Vector2 [] puente_inferior =
    {
        new Vector2(75,40),
        new Vector2(75,44),
        new Vector2(89,40),
        new Vector2(89,44)
    };

    public static float[][] velocidadUnidadPorTerreno =
    {
        new float[MAX_UNIT_TYPE] { 1, 1, 1, 1},
        new float[MAX_UNIT_TYPE] { 1.5f, 1.5f, 2, 2},
        new float[MAX_UNIT_TYPE] { 0.9f, 0.9f, 0.9f, 0.75f},
        new float[MAX_UNIT_TYPE] { 0.5f, 1.25f, 0.5f, 0.5f},    //el arquero es especialista en el bosque
        new float[MAX_UNIT_TYPE] { 0.75f, 0.75f, 0.75f, 0.75f},
        new float[MAX_UNIT_TYPE] { 1f, 1f, 1f, 1f},
    };

    public static Color[] coloresUnidades = { Color.red, Color.blue, Color.black, Color.cyan };
    public static float[] potenciaInfluenciaUnidades = { 10f, 20f, 30f, 40f };
    public static float[] distanciaInfluenciaUnidades = { 5f, 5f, 5f, 5f };
    
    public static float[] influenciaMaximaGeneradaPorUnidad = { 260f, 520f,780f,1040f};

    

    /*BASE INFORMATION*/
    public static float basePotenciaInfluencia = 75f;
    public static float baseDistanciaInfluencia = 15f;
    public static float baseDistaciaCuracion = 37.5f;
    public static float BASE_HEALING_POWER = 20f;
    public const float MAX_BASE_HEALTH = 1000f;

    public static List<Vector3> patrolPathing = new List<Vector3>()
    {
        new Vector3(37.5f,0,262.5f),
        new Vector3(-36.5f,0,262.5f),
        new Vector3(-36.5f,0,337.5f),
        new Vector3(37.5f,0,337.5f),
    };

    /*COMBAT STATS*/
    public static float[] velocidadDeAtaquePorUnidad = { 1f, 1.5f, 0.5f, 0.5f };
    public static float [] healthPerClass = {100f,80f,300f,80f};         //INF - ARQ - PES - MAG
    public static float [] damagePerClass = {10f,5f,10f,30f};
    public static float [] attackRangePerClass = {1.5f,3.5f,1.5f,3.5f};
    public static float [] detectionRangePerClass = {2f,5f,2f,3.5f};
   
    public static float [][] damageModifiers =
    {
                                    //  INF     ARQ     PES     MAG
       /*INF*/ new float[MAX_UNIT_TYPE] {1f,    1.5f,   0.5f,   1f},
       /*ARQ*/ new float[MAX_UNIT_TYPE] {1.5f,  1.5f,   0.5f,   1f},
       /*PES*/ new float[MAX_UNIT_TYPE] {1f,    1f,     1f,     1f},
       /*MAG*/ new float[MAX_UNIT_TYPE] {1f,    1f,     1.5f,   1.5f},

    };


    public static ACCION[][] accionesDeUnidades =
    {
        new ACCION[3]{ACCION.MOVE_ATTACK_OR_FOLLOW, ACCION.FORMATION, ACCION.ROUTE},
        new ACCION[3]{ACCION.MOVE_ATTACK_OR_FOLLOW, ACCION.FORMATION, ACCION.ROUTE},
        new ACCION[2]{ACCION.MOVE_ATTACK_OR_FOLLOW, ACCION.FORMATION},
        new ACCION[2]{ACCION.MOVE_ATTACK_OR_FOLLOW, ACCION.FORMATION}
    };

    public static Color[] coloresTerrenos = { Color.black, Color.gray, Color.green, Color.green + Color.gray, Color.yellow, Color.yellow + Color.red };

    public static Color colorPlayerTeam = Color.blue;
    public static Color colorIATeam = Color.red;

    /*Vector3 a1 = new Vector3(37.5f,0,-37.5f);
            Vector3 a2 = new Vector3(62.5f,0,-37.5f);
            Vector3 a3 = new Vector3(37.5f,0,37.5f);
            Vector3 a4 = new Vector3(62.5f,0,37.5f);

            Vector3 a5 = new Vector3(-62.5f,0,-37.5f);
            Vector3 a6 = new Vector3(-37.5f,0,-37.5f);
            Vector3 a7 = new Vector3(-62.5f,0,37.5f);
            Vector3 a8 = new Vector3(-37.5f,0,37.5f);*/

}
