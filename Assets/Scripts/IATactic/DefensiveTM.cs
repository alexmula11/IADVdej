using System.Collections;
using System.Collections.Generic;
using UnityEngine;


                /*DEFENSIVE TACTICAL MODULE*/
public class DefensiveTM : TacticalModule
{

    public DefensiveTM(Vector2 _baseCoords, List<PersonajeNPC> _npcs, List<PersonajePlayer> _players)
    {
        baseCoords = _baseCoords;
        npcs = _npcs;
        players = _players;
    }
    
    protected internal override List<Accion> getStrategyActions()
    {
        List<Accion> defensiveActions = new List<Accion>();

        foreach(PersonajeNPC npc in npcs)
        {
            //1 -  COMPROBAR SI HAY UNIDADES QUE NECESITEN CURACION
            if(!npc.isFullHealth())
            {
                if(!npc.isInCombat())               //Si no esta en combate
                {
                    ActionGo goToBase = new ActionGo(npc,baseCoords,null);         //TODO realmente debe ir al punto mas cercano dentro del area de curacion de la base
                    defensiveActions.Add(goToBase);
                } 
                else
                {
                    if(npc.betterToRun())           //vida por debajo del 30%
                    {
                         ActionGo goToBase = new ActionGo(npc,baseCoords,null);    //TODO realmente debe ir al punto mas cercano dentro del area de curacion de la base
                         defensiveActions.Add(goToBase);
                    }
                }
            }
            else 
            {
                //2 - COMPROBAR SI HAY ENEMIGOS EN EL AREA DE LA BASE INTERRUMPIENDO SPAWN
                List<PersonajePlayer> enemies = enemiesOnBase();
                if(enemies.Count > 0)
                {
                    PersonajePlayer closestEnemy = getClosestEnemy(npc,players);
                    ActionGo goToEnemy = new ActionGo(npc,closestEnemy.posicion,closestEnemy);
                    AccionAttack attackEnemy = new AccionAttack(npc,closestEnemy);
                    List<Accion> orders = new List<Accion>{goToEnemy,attackEnemy};
                    AccionCompuesta defendBase = new AccionCompuesta(npc,orders,true);
                    defensiveActions.Add(defendBase);
                }
                else
                {
                    //3 -  COMPROBAR UNIDADES FUERA DEL PERIMETRO DE LA BASE
                    if(!isInBaseRange(npc))
                    {
                        ActionGo goToBase = new ActionGo(npc,baseCoords,null);    //TODO realmente debe ir al punto mas cercano dentro del area de curacion de la base
                        defensiveActions.Add(goToBase);
                    }
                    //4 - AGRUPAR UNIDADES DENTRO DEL PERIMETRO DE LA BASE
                }
                
            }
        }
        return defensiveActions;
    }
}
