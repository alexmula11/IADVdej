using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccionPatrullar : Accion
{
    PathFollowingNOPathOffsetGridSD patrullera;
    public AccionPatrullar(PersonajeBase _sujeto, List<Vector3> positions, StatsInfo.TIPO_TERRENO[][] terrenos) : base(_sujeto)
    {
        nombreAccion = "PATRULLANDO";
        patrullera = new PathFollowingNOPathOffsetGridSD(positions, terrenos);
    }

    protected internal override void doit()
    {
        sujeto.newTaskGrid(patrullera);
    }

    protected internal override bool isDone()
    {
        return false;
    }

    protected internal override bool isPossible()
    {
        return sujeto.isAlive();
    }
}
