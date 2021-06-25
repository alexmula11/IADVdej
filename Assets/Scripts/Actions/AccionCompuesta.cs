using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccionCompuesta : Accion
{
    protected internal int actionIndex = 0;
    protected List<Accion> acciones;
    protected bool loop, allDone;


    public AccionCompuesta(PersonajeBase _sujeto, List<Accion> acciones, bool loop) : base(_sujeto)
    {
        this.acciones = new List<Accion>(acciones);
        this.loop = loop;
    }

    protected internal override void doit()
    {
        if (!allDone)
        {
            if (acciones[actionIndex].isDone())
            {
                actionIndex++;
            }

            if (actionIndex >= acciones.Count)
            {
                /*if (loop)
                {
                    actionIndex = 0;
                } 
                else
                {*/
                allDone = true;
                return;
               // }
            }
            else
            {
                while (actionIndex>=0 && !acciones[actionIndex].isPossible())           //intentamos realizar las acciones anteriores
                {
                    actionIndex--;
                }
                if(actionIndex < 0)                                                     //si no es posible, damos la accion por terminada
                {
                    allDone = true;
                }
                else
                {
                    acciones[actionIndex].doit();
                }
            }
           /* while (!acciones[actionIndex].isPossible() && actionIndex>=0)
            {
                actionIndex--;
            }
            if (actionIndex >= 0){
                Debug.Log("realizando accion "+ acciones[actionIndex].nombreAccion);
                acciones[actionIndex].doit();          
            }
            else
            {
                allDone = true;
                loop = false;
            }*/
        }
    }

    protected internal override bool isDone()
    {
        return allDone;
    }

    protected internal override bool isPossible()
    {
        foreach(Accion action in acciones)
        {
            if (action.isPossible())
                return true;
        }
        return false;
    }

    protected internal void actualizeAction()
    {
        if(!allDone && (acciones[actionIndex].isDone() || !acciones[actionIndex].isPossible()))
            doit();
    }
}
