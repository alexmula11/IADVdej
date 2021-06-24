using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TacticalModule
{
  protected internal Vector2 baseCoords;
  protected internal List<PersonajeNPC> npcs;
  protected internal List<PersonajePlayer> players;

  protected internal abstract List<Accion> getStrategyActions();

  protected internal bool isInBaseRange(PersonajeNPC npc)                                                          //Comprobar si un npc esta en el area de defensa de la base
    {
        return (new Vector2(npc.posicion.x,npc.posicion.y)-baseCoords).magnitude <= StatsInfo.baseDistaciaCuracion;
    }

  protected internal List<PersonajePlayer> enemiesOnBase()                                                        //comprobar si hay enemigos en la base atacando
  {
      List<PersonajePlayer> enemies = new List<PersonajePlayer>();
      foreach(PersonajePlayer pp in players)
        if((new Vector2(pp.posicion.x,pp.posicion.y)-baseCoords).magnitude <= StatsInfo.baseDistaciaCuracion)
            enemies.Add(pp);    
      return enemies;
   }

  protected internal bool ourBaseIsUnderAttack()
    {
      foreach(PersonajePlayer pp in players)
        if((new Vector2(pp.posicion.x,pp.posicion.y)-baseCoords).magnitude <= StatsInfo.baseDistaciaCuracion)
            return true;    
      return false;
    }
 protected internal PersonajePlayer getClosestEnemy(PersonajeNPC npc, List<PersonajePlayer> enemies)              //seleccionar el enemigo mas cercano
   {
     PersonajePlayer closestEnemy = null;
     float minDist = float.MaxValue;

      foreach(PersonajePlayer pp in enemies)
          if((npc.posicion-pp.posicion).magnitude < minDist)
              closestEnemy = pp;     
     return closestEnemy; 
    }

}
