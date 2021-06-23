using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGo : Accion
{
    private PathFollowEndSD recorrer;

    public ActionGo(PersonajeBase _sujeto, Vector2 destiny) : base( _sujeto)
    {
        nombreAccion = "MOVER";
        sujeto = _sujeto;
        List<Vector3> camino = SimManagerFinal.aStarPathV3(SimManagerFinal.positionToGrid(_sujeto.posicion),destiny,_sujeto.tipo);
        recorrer = new PathFollowEndSD(camino);
    }

    protected internal override void doit()
    {
        sujeto.newTask(recorrer);
    }

    protected internal override bool isDone()
    {
        return false;
    }
}
