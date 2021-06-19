using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarSD : SteeringBehaviour
{
    protected StatsInfo.TIPO_TERRENO[][] terrenos;
    protected float[][] pesos;
    protected Vector2 origen, destino;
    protected bool setup=false;
    protected LinkedList<Vector2> recorrido = new LinkedList<Vector2>();


    public AStarSD(StatsInfo.TIPO_TERRENO[][] terrenos, Vector2 origen, Vector2 destino)
    {
        this.terrenos = terrenos;
        this.origen = origen;
        this.destino = destino;
        pesos = new float[terrenos.Length][];
        for (int i=0; i < pesos.Length; i++)
        {
            pesos[i] = new float[terrenos[i].Length];
        }
    }






    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        if (!setup)
        {
            LinkedList<Vector2> closedPositions = new LinkedList<Vector2>();
            LinkedList<Vector2> openPositions = new LinkedList<Vector2>();

            while (!setup)
            {

            }

            setup = true;
        }
        
        throw new System.NotImplementedException();
    }
}
