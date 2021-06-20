using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AStarSD : SteeringBehaviour
{
    protected StatsInfo.TIPO_TERRENO[][] terrenos;
    //protected float[][] distancias;
    protected int pasoActual=0;
    protected Vector2 origen, destino;
    protected NodoGrafoAStar nodoOrigen, nodoDestino;
    protected bool setup=false;
    protected LinkedList<Vector2> recorrido = new LinkedList<Vector2>();
    protected PursueSD pursue = new PursueSD();



    public AStarSD(StatsInfo.TIPO_TERRENO[][] terrenos, Vector2 origen, Vector2 destino)
    {
        this.terrenos = terrenos;
        this.origen = origen;
        this.destino = destino;/*
        distancias = new float[terrenos.Length][];
        for (int i=0; i < distancias.Length; i++)
        {
            distancias[i] = new float[terrenos[i].Length];
        }*/
        float estimatedCost = (destino - origen).magnitude;
        nodoOrigen = new NodoGrafoAStar(origen,0f,null);
    }

    public LinkedList<NodoGrafoAStar> calcularAdyacentes(NodoGrafoAStar actual, StatsInfo.TIPO_PERSONAJE type)
    {
        LinkedList<NodoGrafoAStar> listanodos = new LinkedList();

        for(int i=-1;i<2;i++)
        {
            for (int j=-1,j<2;j++)
            {
                if (i!=0 || j!=0)
                {
                    Vector2 newPosi = new Vector2(actual.posicionGrid.x+i, actual.posicionGrid.y+j);
                    if(terrenos[newPosi.x][newPosi.y]!=StatsInfo.TIPO_TERRENO.INFRANQUEABLE)
                    {
                        float inversaVelocidad = 1/StatsInfo.velocidadUnidadPorTerreno[terrenos[newPosi.x][newPosi.y]][type];
                        float newG=actual.costFromOrigin + (destino - newPosi).magnitude*inversaVelocidad;
                        listanodos.Add(new NodoGrafoAStar(newPosi, (destino - newPosi).magnitude,newG,actual);
                    }

                }
            }
        }
        return listanodos;
    }






    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        if (!setup)
        {
            LinkedList<NodoGrafoAStar> closedPositions = new LinkedList<NodoGrafoAStar>();
            closedPositions.Add(nodoOrigen);
            LinkedList<NodoGrafoAStar> openPositions = new LinkedList<NodoGrafoAStar>();
            

            NodoGrafoAStar nodoActual = nodoOrigen;
            while (!setup)
            {
                LinkedList<NodoGrafoAStar> adyacentes = calcularAdyacentes(nodoActual,personaje.tipo);
                //LinkedList<NodoGrafoAStar> adyacentesFiltrados 
                foreach(NodoGrafoAStar nodito in adyacentes)
                {
                    //Observamos lista closed
                    bool estaEnListaClosed = false;
                    foreach (NodoGrafoAStar noditoClosed in closedPositions)
                    {
                        //Si el nodo ya está en la lista closed, no se considera
                        if (noditoClosed.posicionGrid==nodito.posicionGrid)
                        {
                            estaEnListaClosed = true;
                            continue;
                        }
                    }
                    if (estaEnListaClosed)
                    {
                        break;
                    }

                    //Observamos lista open
                    NodoGrafoAStar posibleaASustituir = null;
                    bool estaEnListaOpen = false;
                    foreach(NodoGrafoAStar noditoOpen in openPositions)
                    {
                        if (noditoOpen.posicionGrid==noditoOpen.posicionGrid) 
                        {
                            estaEnListaOpen = true;
                            if (noditoOpen.totalCost>nodito.totalCost)
                            {
                                posibleaASustituir = noditoOpen;
                            }
                            break;
                        }
                    }
                    if (posibleaASustituir!=null)
                    {
                        openPositions.Remove(posibleaASustituir);
                        openPositions.Add(nodito);
                    }
                    elseif (!estaEnListaOpen)
                    {
                        openPositions.Add(nodito);
                    }
                }
                //Calculamos siguiente nodo
                float minorCost = float.MaxValue;
                NodoGrafoAStar next = null;
                foreach(NodoGrafoAStar noditoOpen in openPositions)
                {
                    if(noditoOpen.totalCost < minorCost)
                    {
                        minorCost = noditoOpen.totalCost;
                        next = noditoOpen;
                    }
                }
                nodoActual = next;
                closedPositions.Add(nodoActual);
                //Comprobacion de parada(llegamos al destina)
                foreach (NodoGrafoAStar noditoClosed in closedPositions)
                {
                    if (noditoClosed.posicionGrid == destino)
                    {
                        setup=true;
                    }
                }

            }
            //Calculamos el camino a seguir en base a los padres del nodo destino
            NodoGrafoAStar aux = nodoActual;
            while(aux.padre!=null)
            {
                recorrido.AddFirst(aux.posicionGrid);
            }
        }
        
        if (pursue.finishedLinear)
        {
            if (pasoActual >= recorrido.Count-1)
            {
                _finishedLinear = true;
                _finishedAngular = true;
                return new Steering();
            }
            else
            {
                pasoActual++;
                personaje.fakeMovement.posicion = SimManagerFinal.gridToPosition(recorrido.get(pasoActual));
                personaje.fakeMovement.moveTo(SimManagerFinal.gridToPosition(recorrido.get(pasoActual)));
                pursue.target=personaje.fakeMovement;
                return pursue.getSteering(personaje);
            }
        }
        else
        {
            return pursue.getSteering(personaje);
        } 
    


    }
}
