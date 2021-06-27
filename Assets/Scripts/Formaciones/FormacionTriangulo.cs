using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

//Esta formacion está formada por 4 jugadores
//El lider se ubica en una posicion determinada por el jugador 
//Los 3 otros miembros forman fila detras de el
public class FormacionTriangulo : Formacion
{
    public FormacionTriangulo(PersonajeBase lider) :base(lider,4){
        for (int i = 0; i < maximoMiembros-1; i++)
        {
            offsetPositions[i] = new Vector3((-10f + (i * 10f)), 0, -10f);//Valores de z para el segundo vector -> -5 0 5
        }
        for (int i = 0; i < maximoMiembros-1; i++)
        {
            offsetRotations[i] = 0;
        }
    }
    /*
     * Cosas que cambio:
     * 1.- Las posiciones no se calculan, están ya predefinidas en una lista
     * 2.- No creamos personajes fake, cada personaje tiene sus propios fakes accesibles, es más limpio y controlable y así se está haciendo, pero de hecho
     *      aquí le hacemos OffsetPursuit al lider directamente (y el offset pursuit ya usa los fakes)
     * 3.- Cambié los PursueSD de los que no son líderes por OffsetPursuitSD
     * 4.- Inicializador de lista de los miembros
     * 5.- También puse lider como accesor al primer elemento de la lista de miembros, ya que el código es dependiente de esto, por lo que siempre mantendremos al líder al principio de esta lista
     */
}
