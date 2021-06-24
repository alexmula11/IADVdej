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
            if ((receptor.posicion - sujeto.posicion).magnitude > StatsInfo.attackRangePerClass[(int)sujeto.tipo])
            {
                List<Vector3> camino = SimManagerFinal.aStarPathV3(SimManagerFinal.positionToGrid(sujeto.posicion), destiny, sujeto.tipo);
                recorrer = new PathFollowEndSD(camino);
            }
        }
        else
        {
            List<Vector3> camino = SimManagerFinal.aStarPathV3(SimManagerFinal.positionToGrid(sujeto.posicion), destiny, sujeto.tipo);
            recorrer = new PathFollowEndSD(camino);
            sujeto.newTask(recorrer);
        }
    }

    protected internal override bool isDone()
    {
        return (receptor && (receptor.posicion - sujeto.posicion).magnitude <= StatsInfo.attackRangePerClass[(int)sujeto.tipo]) || recorrer.finishedLinear;
    }

    protected internal override bool isPossible()
    {
        return (receptor && receptor.isAlive() && sujeto.isAlive()) || (!receptor && sujeto.isAlive());
    }
}
