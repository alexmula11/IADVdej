using System.Collections;
using System.Collections.Generic;
using UnityEngine;


                /*DEFENSIVE TACTICAL MODULE*/
public class DefensiveTM : TacticalModule
{

    private bool defended = false;
    public DefensiveTM(Vector2 _baseCoords, List<PersonajeBase> _npcs, List<PersonajeBase> _players) : base(_baseCoords, _npcs, _players)
    {
    }

    protected internal override List<Accion> getStrategyActions()
    {
        List<Accion> defensiveActions = new List<Accion>();

        foreach(PersonajeBase ally in allies)
        {
            //1 -  COMPROBAR SI HAY UNIDADES QUE NECESITEN CURACION
            if(!ally.isFullHealth())
            {
                if(!ally.isInCombat())               //Si no esta en combate
                {
                    if (!isInBaseRange(ally))
                    {
                        Vector2 closestPoint = getClosestPointToBase(ally, baseCoords);
                        ActionGo goToBase = new ActionGo(ally, closestPoint, null);
                        defensiveActions.Add(goToBase);
                    }
                } 
                else
                {
                    if(ally.betterToRun())           //vida por debajo del 30%
                    {
                        if (!isInBaseRange(ally))
                        {
                            Vector2 closestPoint = getClosestPointToBase(ally, baseCoords);
                            ActionGo goToBase = new ActionGo(ally, closestPoint, null);
                            defensiveActions.Add(goToBase);
                        }
                    }
                }
            }
            else 
            {
                //2 - COMPROBAR SI HAY ENEMIGOS EN EL AREA DE LA BASE INTERRUMPIENDO SPAWN
                List<PersonajeBase> enemies_attacking = enemiesOnBase();
                if(enemies_attacking.Count > 0)
                {
                    PersonajeBase closestEnemy = getClosestEnemy(ally, enemies_attacking);
                    ActionGo goToEnemy = new ActionGo(ally,SimManagerFinal.positionToGrid(closestEnemy.posicion),closestEnemy);
                    AccionAttack attackEnemy = new AccionAttack(ally,closestEnemy);
                    List<Accion> orders = new List<Accion>{goToEnemy,attackEnemy};
                    AccionCompuesta defendBase = new AccionCompuesta(ally,orders,true);
                    defensiveActions.Add(defendBase);
                    defended = true;
                }
                else
                {
                    //3 -  COMPROBAR UNIDADES FUERA DEL PERIMETRO DE LA BASE
                    if(!isInBaseRange(ally))
                    {
                        Vector2 closestPoint = getClosestPointToBase(ally, baseCoords);
                        ActionGo goToBase = new ActionGo(ally, closestPoint, null);
                        defensiveActions.Add(goToBase);
                    }
                    //4 - AGRUPAR UNIDADES DENTRO DEL PERIMETRO DE LA BASE
                }
                
            }
        }
        return defensiveActions;
    }
}
