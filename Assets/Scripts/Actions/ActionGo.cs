using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGo : Accion
{
    private PathFollowEndSD recorrer;
    private PersonajeBase receptor;
    private Vector2 destiny;

    public ActionGo(PersonajeBase _sujeto, Vector2 destiny, PersonajeBase receptor) : base( _sujeto)
    {
        if (receptor)
        {
            nombreAccion = "PERSEGUIR";
            this.receptor = receptor;
        }
        else
        {
            nombreAccion = "MOVER";
        }
        this.destiny = destiny;
    }

    protected internal override void doit()
    {
        List<Vector3> camino = SimManagerFinal.aStarPathV3(SimManagerFinal.positionToGrid(sujeto.posicion), destiny, sujeto.tipo);
        recorrer = new PathFollowEndSD(camino);
        sujeto.newTask(recorrer);
    }

    protected internal override bool isDone()
    {
        //if(receptor != null)Debug.Log("Distancia al enemigo "+ (receptor.posicion - sujeto.posicion).magnitude + " rango de ataque "+ StatsInfo.attackRangePerClass[(int)sujeto.tipo] + "receptor "+ receptor.posicion + "sujeto "+ sujeto.posicion);
        //if(receptor!=null && (receptor.posicion - sujeto.posicion).magnitude < StatsInfo.attackRangePerClass[(int)sujeto.tipo])Debug.Log("ya estoy a rango");
        return (receptor && (receptor.posicion - sujeto.posicion).magnitude < StatsInfo.attackRangePerClass[(int)sujeto.tipo]) || (recorrer!=null && recorrer.finishedLinear);
    }

    protected internal override bool isPossible()
    {
        return (receptor && receptor.isAlive() && sujeto.isAlive()) || (receptor==null && sujeto.isAlive());
    }
}
