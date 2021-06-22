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
     private AggresiveTM attacker;
     private DefensiveTM defender;
     private IA_MODE playingMode;


    public TacticIA(List<PersonajeNPC> _npcs)
    {
        npcs = _npcs;
    }

    public void playIA()
    {
        //bucle de juego
        //segun el modo de juego
        switch(playingMode)
        {
            case IA_MODE.ATTACK:
            break;
            case IA_MODE.DEFEND:
            break;
            case IA_MODE.TOTAL_WAR:
            break;
        }
        //calcula acciones
        //ejecuta acciones
    }


     public void change_IA_Mode(IA_MODE new_mode)
     {
         playingMode = new_mode;
     }
}
