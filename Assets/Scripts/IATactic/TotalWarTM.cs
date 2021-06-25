using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalWarTM : TacticalModule
{
    public TotalWarTM(Vector2 _baseCoords, List<PersonajeBase> _npcs, List<PersonajeBase> _players) : base(_baseCoords, _npcs, _players)
    {
    }

    protected internal override List<Accion> getStrategyActions()
    {
        List<Accion> totalWarActions = new List<Accion>();
        //1 - mandar a por la base enemiga
        //2 - si encuentra enemigos nos damos de palos
        return totalWarActions;
    }
}
