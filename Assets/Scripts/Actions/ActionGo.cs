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
        if (receptor)
        {
            destiny = SimManagerFinal.positionToGrid(receptor.posicion);
        }
        List<Vector3> camino = SimManagerFinal.aStarPathV3(SimManagerFinal.positionToGrid(sujeto.posicion), destiny, sujeto.tipo, sujeto is PersonajePlayer);
        recorrer = new PathFollowEndSD(camino);
        sujeto.newTaskGrid(recorrer);
    }

    protected internal override bool isDone()
    {
        bool aRango = false;
        if(receptor != null){
            Vector2 mipos = SimManagerFinal.positionToGrid(sujeto.posicion);
            Vector2 supos = SimManagerFinal.positionToGrid(receptor.posicion);
            aRango = (mipos - supos).magnitude <= (StatsInfo.attackRangePerClass[(int)sujeto.tipo]);
        }
        
        return (aRango || (recorrer!=null && recorrer.finishedLinear));
    }

    protected internal override bool isPossible()
    {
        return (receptor && receptor.isAlive() && sujeto.isAlive()) || (receptor==null && sujeto.isAlive());
    }

    protected internal Vector2 getDestiny()
    {
        return destiny;
    }

    protected internal override bool hasToRecalculate()
    {
        return (receptor && destiny!= SimManagerFinal.positionToGrid(receptor.posicion));
    }
}
