using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum THREAT_VALUE
{
    LOW,
    MEDIUM,
    HIGH
}

public abstract class TacticalModule
{

  protected internal const int LOWDANGER = 3;
  protected internal const int MIDDANGER = 7;
  protected internal const int HIGHDANGER = 10;

  protected internal Vector2 baseCoords;
  protected internal Vector2 enemyBaseCoords;
  protected internal List<PersonajeBase> allies;
  protected internal List<PersonajeBase> enemies;

  protected internal bool team;

  

  protected internal abstract List<Accion> getStrategyActions();



    public TacticalModule(Vector2 _baseCoords, Vector2 _enemyBaseCoords ,List<PersonajeBase> alice, List<PersonajeBase> nemy, bool _team)
    {
      team = _team;
      baseCoords = _baseCoords;
      enemyBaseCoords = _enemyBaseCoords;
      allies = alice;
      enemies = nemy;
    }

      protected static internal bool isInBaseRange(PersonajeBase person, Vector2 baseCoord)                                                          
        {
            return (person.posicion - SimManagerFinal.gridToPosition(baseCoord)).magnitude <= StatsInfo.baseDistaciaCuracion;
        }

      protected internal List<PersonajeBase> enemiesOnBase()                                                        //comprobar si hay enemigos en la base atacando
      {
          List<PersonajeBase> enemies_attacking = new List<PersonajeBase>();
          foreach(PersonajeBase pp in enemies)
            if(pp.isAlive() && ((pp.posicion - SimManagerFinal.gridToPosition(baseCoords)).magnitude <= StatsInfo.baseDistaciaCuracion))
                enemies_attacking.Add(pp);    
          return enemies_attacking;
       }

      protected internal bool ourBaseIsUnderAttack()
        {
          foreach(PersonajeBase pp in enemies)
            if(pp.isAlive() && ((pp.posicion-SimManagerFinal.gridToPosition(baseCoords)).magnitude <= StatsInfo.baseDistaciaCuracion))
                return true;    
          return false;
        }
     protected internal PersonajeBase getClosestEnemy(PersonajeBase npc, List<PersonajeBase> enemies_att)              //seleccionar el enemigo mas cercano
       {
         PersonajeBase closestEnemy = null;
         float minDist = float.MaxValue;

          foreach(PersonajeBase pp in enemies_att)
              if((npc.posicion-pp.posicion).magnitude < minDist)
              {
                minDist = (npc.posicion-pp.posicion).magnitude;
                closestEnemy = pp;
              }                      
         return closestEnemy; 
        }


    protected internal Vector2 getUnitPointOnBase(PersonajeBase person, Vector2 basePos)
    {
        int index = 0;
        foreach (PersonajeBase ppl in allies)
        {
            if (person == ppl)
            {
                break;
            }
            index++;
        }
        int angle = index *360/allies.Count;

        Vector3 radio = new Vector3((float)System.Math.Cos(angle), 0,(float)System.Math.Sin(angle)) * StatsInfo.baseDistaciaCuracion*0.75f;

        Vector3 destino = SimManagerFinal.gridToPosition(basePos) + radio;
        return SimManagerFinal.positionToGrid(destino);
        


        /*Vector3 distance = SimManagerFinal.gridToPosition(basePos) - person.posicion;
        if (distance.magnitude < StatsInfo.baseDistaciaCuracion)
        {
            return SimManagerFinal.positionToGrid(person.posicion);
        }
        else
        {
            distance = distance.normalized* (distance.magnitude  - StatsInfo.baseDistaciaCuracion + 5);
            return SimManagerFinal.positionToGrid(person.posicion + distance);
        }*/
    }
    protected static internal Vector2 getUnitPointOnBaseStatic(PersonajeBase person, Vector2 basePos, List<PersonajeBase> alice)
    {
        int index = 0;
        foreach (PersonajeBase ppl in alice)
        {
            if (person == ppl)
            {
                break;
            }
            index++;
        }
        int angle = index * 360 / alice.Count;

        Vector3 radio = new Vector3((float)System.Math.Cos(angle), 0, (float)System.Math.Sin(angle)) * StatsInfo.baseDistaciaCuracion * 0.75f;

        Vector3 destino = SimManagerFinal.gridToPosition(basePos) + radio;
        return SimManagerFinal.positionToGrid(destino);
    }


    protected static internal AccionCompuesta createAttackingAction(PersonajeBase sujeto, PersonajeBase receptor)
    {
      ActionGo goToEnemy = new ActionGo(sujeto,SimManagerFinal.positionToGrid(receptor.posicion),receptor);
      AccionAttack attackEnemy = new AccionAttack(sujeto,receptor);
      List<Accion> orders = new List<Accion>{goToEnemy,attackEnemy};
      AccionCompuesta ac = new AccionCompuesta(sujeto,orders,true);
      return ac;
    }

    protected internal ActionGo createBaseAttackAction(PersonajeBase sujeto)
    {
      ActionGo goToEnemyBase = new ActionGo(sujeto,getUnitPointOnBase(sujeto,enemyBaseCoords),null);
      return goToEnemyBase;
    }

    protected internal ActionGo goingToRecover(PersonajeBase sujeto)
    {
      ActionGo goToMyBase = new ActionGo(sujeto,getUnitPointOnBase(sujeto,baseCoords),null);
      return goToMyBase;
    }

    protected internal bool isGoingToAttack(PersonajeBase unit)
    {
      return (unit.currentAction is AccionCompuesta);
    }

    protected internal bool isGoingToEnemyBase(PersonajeBase person)
    {
        return (person.currentAction is ActionGo && (enemyBaseCoords - (person.currentAction as ActionGo).getDestiny()).magnitude <= StatsInfo.baseDistaciaCuracion);
    }

    protected internal  List<Vector3> getPatrolPathing(bool team)
    {
        if(team)
        {
            return StatsInfo.patrolPathingBLUE;
        }
        else
        {
            return StatsInfo.patrolPathingRED;
        }
    }

    protected internal bool alreadyComingToBase(PersonajeBase unit)
    {

        Vector3 destino = Vector3.zero;
        if (unit.currentAction != null && unit.currentAction is ActionGo)
        {
            destino = SimManagerFinal.gridToPosition(((ActionGo)unit.currentAction).getDestiny());
        }
        return (destino - SimManagerFinal.gridToPosition(baseCoords)).magnitude <= StatsInfo.baseDistaciaCuracion;
    }

    protected abstract internal void tioMuerto(PersonajeBase tio);


}
