﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


                    /*AGGRESIVE TACTICAL MODULE*/
public class AggresiveTM : TacticalModule
{

    public AggresiveTM(Vector2 _baseCoords, List<PersonajeNPC> _npcs, List<PersonajePlayer> _players)
    {
        baseCoords = _baseCoords;
        npcs = _npcs;
        players = _players;
    }

    protected internal override List<Accion> getStrategyActions()
    {
        List<Accion> aggresiveActions = new List<Accion>();
        
        //Comprobar estado de defensa de nuestra base
        //si hay enemigos atacando la bse
        //ir a defenderla
        //controlar puentes
        //ataques coordinados
        return null;
    }
}
