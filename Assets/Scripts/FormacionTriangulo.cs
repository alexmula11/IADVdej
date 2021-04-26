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
        for (int i = 1; i < maximoMiembros; i++)
        {
            offsetPositions.Add(new Vector3(5, 0, (-5 + ((i - 1) * 5))));//Valores de z para el segundo vector -> -5 0 5
        }
    }

    internal override void formacionASusPuestos(Vector3 posicionLider)
    {   
        PersonajeNPC fake = new PersonajeNPC();
        fake.posicion = posicionLider;
        PursueSD p = new PursueSD();
        p.target = fake;

        lider.newTask(p);

        for(int i = 1; i<maximoMiembros;i++)
        {
            OffsetPursuitSD opSD = new OffsetPursuitSD(offsetPositions[i - 1]);
            opSD.target = lider;
            miembros[i].newTask(p);
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

    internal override void formacionEnAccion(Vector3 posicionLider)
    {   
        PersonajeNPC fake = new PersonajeNPC();
        fake.posicion = posicionLider;
        PursueSD p = new PursueSD();
        p.target = fake;

        lider.newTask(p);


        //Para detener al lider cada 5 segundos
        Stopwatch stopwatch = new Stopwatch();
        if (stopwatch.ElapsedMilliseconds%5000 == 0)
        {
            //lider.stop();
            stopwatch.Restart();
        }

        //Igualamos la velocidad y aceleracion de los secuaces al lider
        for(int i = 1; i<maximoMiembros;i++)
        {
            //miembros[i].maxMovSpeed = lider.maxMovSpeed;
            //miembros[i].movementAccel = lider.movementAccel;
        }

        for(int i = 1; i<maximoMiembros;i++)
        {
            PersonajeNPC fake2 = new PersonajeNPC();
            fake2.posicion = posicionLider-new Vector3(5,0,(-5+((i-1)*5)));//Valores de z para el segundo vector -> -5 0 5
            p = new PursueSD();
            p.target = fake2;
            miembros[i].newTask(p);
        }
    }


}
