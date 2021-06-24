using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalWarTM : TacticalModule
{
   public TotalWarTM(Vector2 _baseCoords, List<PersonajeNPC> _npcs, List<PersonajePlayer> _players)
    {
        baseCoords = _baseCoords;
        npcs = _npcs;
        players = _players;
    }

    protected internal override List<Accion> getStrategyActions()
    {
        List<Accion> totalWarActions = new List<Accion>();
        //1 - mandar a por la base enemiga
        //2 - si encuentra enemigos nos damos de palos
        return totalWarActions;
    }
}
