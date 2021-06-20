using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsInfo
{
    public enum TIPO_PERSONAJE
    {
        INFANTERIA,
        ARQUERO,
        PESADA,
        MAJITO
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
        MOVE_OR_FOLLOW,
        FORMATION,
        ROUTE
    }


    public static float[] velocidadUnidades =
    {
        3, 4, 2, 2
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
    public static float[][] velocidadUnidadPorTerreno =
    {
        new float[4] { 1, 1, 1, 1},
        new float[4] { 1.5f, 1.5f, 2, 2},
        new float[4] { 1, 1, 1, 1},
        new float[4] { 0.66f, 0.75f, 0.5f, 0.5f},
        new float[4] { 0.66f, 0.66f, 0.5f, 0.5f},
        new float[4] { 1.5f, 1.5f, 1.5f, 1.5f},
    };
    public static Color[] coloresUnidades = { Color.red, Color.blue, Color.black, Color.cyan };

    public static ACCION[][] accionesDeUnidades =
    {
        new ACCION[3]{ACCION.MOVE_OR_FOLLOW, ACCION.FORMATION, ACCION.ROUTE},
        new ACCION[3]{ACCION.MOVE_OR_FOLLOW, ACCION.FORMATION, ACCION.ROUTE},
        new ACCION[2]{ACCION.MOVE_OR_FOLLOW, ACCION.FORMATION},
        new ACCION[2]{ACCION.MOVE_OR_FOLLOW, ACCION.FORMATION}
    };

}
