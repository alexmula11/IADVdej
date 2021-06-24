using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum IA_MODE
{
    ATTACK,
    DEFEND,
    TOTAL_WAR
}

public class TacticIA
{
     private List<PersonajeNPC> npcs;
     private List<PersonajePlayer> players;
     
     private TacticalModule comander;
     private IA_MODE playingMode;

     private Vector2 baseCoords;
     private bool baseUnderAttack = false;


    public TacticIA(List<PersonajeNPC> _npcs, List<PersonajePlayer> _players, IA_MODE mode, Vector2 _baseCoords)
    {
        npcs = _npcs;
        players = _players;
        playingMode = mode;
        baseCoords = _baseCoords;
        comander = factoryTM(playingMode);
    }

    public void playIA()
    {
        List<Accion> orders = comander.getStrategyActions();

        foreach(Accion ord in orders)
        {
            if(ord.sujeto.currentAction != ord)                     //si es la misma orden no hace falta machacarla
            {
                ord.sujeto.currentAction = ord;
                ord.sujeto.currentAction.doit();
            }
        }
        baseUnderAttack = comander.ourBaseIsUnderAttack();
        if(!baseUnderAttack)                                        //si no estamos siendo atacados las curaciones y respawn estan activos
        {
            //respawn de unidades muertas
             //TODO realizar curaciones
        }     
    }


     public void change_IA_Mode(IA_MODE new_mode)
     {
         if(playingMode != new_mode)
         {
             playingMode = new_mode;
             comander = factoryTM(playingMode);
         }     
     }

     private TacticalModule factoryTM(IA_MODE mode)
     {
         TacticalModule tm = null;
         switch(mode)
        {
            case IA_MODE.ATTACK:
                 tm = new AggresiveTM(baseCoords,npcs,players);
            break;
            case IA_MODE.DEFEND:
                 tm = new DefensiveTM(baseCoords,npcs,players);
            break;
            case IA_MODE.TOTAL_WAR:
                tm = new TotalWarTM(baseCoords,npcs,players);
            break;
        }
        return tm;
     }
}
