using System.Collections;
using System.Collections.Generic;
using UnityEngine;


                /*DEFENSIVE TACTICAL MODULE*/
public class DefensiveTM : TacticalModule
{

    protected internal override List<Accion> getStrategyActions(List<PersonajeNPC> npcs)
    {
        List<Accion> defensiveActions = new List<Accion>();

        //1 -  COMPROBAR SI HAY UNIDADES QUE NECESITEN CURACION
        foreach(PersonajeNPC p_npc in npcs)
        {
            if(!p_npc.isFullHealth())
            {
                /* if(combatManager.isBeingAttacked(npc))
                    //añadir un ataque
                    else
                    ir a la base
                */
            }
        }

        //2 -  COMPROBAR UNIDADES FUERA DEL PERIMETRO DE LA BASE

        //3 - COMPROBAR SI HAY ENEMIGOS EN EL AREA DE LA BASE INTERRUMPIENDO SPAWN

        return defensiveActions;
    }

    private bool isInBaseRange(PersonajeNPC npc)                                    //Comprobar si un npc esta en el area de defensa de la base
    {
        return false;
    }

    private bool isInBaseHealingRange(PersonajeNPC npc)                             //comprobar si un npc esta en el rango de curacion de la base
    {
        return false;
    }
}
