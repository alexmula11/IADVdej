using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TacticalModule
{
  protected internal Vector2 baseCoords;
  protected internal List<PersonajeBase> allies;
  protected internal List<PersonajeBase> enemies;

  protected internal abstract List<Accion> getStrategyActions();

    public TacticalModule(Vector2 _baseCoords, List<PersonajeBase> alice, List<PersonajeBase> nemy)
    {
        baseCoords = _baseCoords;
        allies = alice;
        enemies = nemy;
    }

      protected internal bool isInBaseRange(PersonajeBase person)                                                          
        {
            return (person.posicion - SimManagerFinal.gridToPosition(baseCoords)).magnitude <= StatsInfo.baseDistaciaCuracion;
        }

      protected internal List<PersonajeBase> enemiesOnBase()                                                        //comprobar si hay enemigos en la base atacando
      {
          List<PersonajeBase> enemies = new List<PersonajeBase>();
          foreach(PersonajeBase pp in this.enemies)
            if((pp.posicion - SimManagerFinal.gridToPosition(baseCoords)).magnitude <= StatsInfo.baseDistaciaCuracion)
                enemies.Add(pp);    
          return enemies;
       }

      protected internal bool ourBaseIsUnderAttack()
        {
          foreach(PersonajeBase pp in enemies)
            if((SimManagerFinal.positionToGrid(pp.posicion)-baseCoords).magnitude <= StatsInfo.baseDistaciaCuracion)
                return true;    
          return false;
        }
     protected internal PersonajeBase getClosestEnemy(PersonajeBase npc, List<PersonajeBase> enemies)              //seleccionar el enemigo mas cercano
       {
            PersonajeBase closestEnemy = null;
         float minDist = float.MaxValue;

          foreach(PersonajeBase pp in enemies)
              if((npc.posicion-pp.posicion).magnitude < minDist)
                  closestEnemy = pp;     
         return closestEnemy; 
        }

    protected internal Vector2 getClosestPointToBase(PersonajeBase person, Vector2 basePos)
    {

        Vector3 distance = SimManagerFinal.gridToPosition(basePos) - person.posicion;
        if (distance.magnitude < StatsInfo.baseDistaciaCuracion)
        {
            return SimManagerFinal.positionToGrid(person.posicion);
        }
        else
        {
            distance = distance.normalized* (distance.magnitude  - StatsInfo.baseDistaciaCuracion + 5);
            return SimManagerFinal.positionToGrid(person.posicion + distance);
        }
    }
}
