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
     private List<PersonajeBase> allies;
     private List<PersonajeBase> enemies;
     
     private TacticalModule comander;
     private IA_MODE playingMode;

     private Vector2 allyBasePos, enemyBasePos;
     private bool baseUnderAttack = false;
     private float baseHealth = StatsInfo.MAX_BASE_HEALTH;


    public TacticIA(List<PersonajeBase> allis, List<PersonajeBase> enemis, Vector2 allyBase, Vector2 enemyBase)
    {
        allies = allis;
        enemies = enemis;
        playingMode = IA_MODE.ATTACK;
        allyBasePos = allyBase;
        comander = factoryTM(playingMode);
    }

    public void playIA()
    {
        List<Accion> orders = comander.getStrategyActions();

        foreach(Accion ord in orders)
        {
            ord.sujeto.currentAction = ord; //TODO comprobar que no es la misma
            ord.sujeto.currentAction.doit();
        }
        baseUnderAttack = comander.ourBaseIsUnderAttack();
        if(!baseUnderAttack)                                        //si no estamos siendo atacados las curaciones y respawn estan activos
        {
            //respawn de unidades muertas
             //TODO realizar curaciones
        }     
    }

    //private 

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
                 tm = new AggresiveTM(allyBasePos,allies,enemies);
            break;
            case IA_MODE.DEFEND:
                 tm = new DefensiveTM(allyBasePos,allies,enemies);
            break;
            case IA_MODE.TOTAL_WAR:
                tm = new TotalWarTM(allyBasePos,allies,enemies);
            break;
        }
        return tm;
     }
}
