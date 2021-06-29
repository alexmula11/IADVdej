using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalWarTM : TacticalModule
{
    private List<PersonajeBase> attackGroup;

    private List<PersonajeBase> unitsNotAsigned;

    public TotalWarTM(Vector2 _baseCoords, Vector2 _enemyBaseCoords, List<PersonajeBase> _npcs, List<PersonajeBase> _players, bool _team) : base(_baseCoords, _enemyBaseCoords ,_npcs, _players, _team)
    {
        attackGroup = new List<PersonajeBase>();
        unitsNotAsigned = new List<PersonajeBase>();
    }

    protected internal override List<Accion> getStrategyActions()
    {
        List<Accion> totalWarActions = new List<Accion>();

        //1 - rellenamos la lista de unidades operativas
        unitsNotAsigned.Clear();
        checkUnitsAvailable();  //no añadimos los que ya han sido mandados a darse de palos

        //2 - si encuentramos enemigos nos damos de palos
        totalWarActions.AddRange(attackEnemiesClose());

        //3 - mandar a por la base enemiga
        totalWarActions.AddRange(attackEnemyBase());

        //4 - mandamos rezagados
        totalWarActions.AddRange(rezagadosAttack());
        return totalWarActions;
    }


    private void checkUnitsAvailable()
    {
        foreach(PersonajeBase unit in allies)
        {
            if(unit.isAlive())
            {
                if(!isGoingToAttack(unit))
                {
                    unitsNotAsigned.Add(unit);   
                }
                else
                {
                    attackGroup.Remove(unit);
                }                        
            }  
            else
            {
                attackGroup.Remove(unit);
            }            
        }
    }

    private List<Accion> attackEnemiesClose()
    {
        List<Accion> attackActions = new List<Accion>();

        foreach(PersonajeBase unit in unitsNotAsigned)
        {
            foreach(PersonajeBase enemy in enemies)
            {               
                Vector2 mipos = SimManagerFinal.positionToGrid(unit.posicion);
                Vector2 supos = SimManagerFinal.positionToGrid(enemy.posicion); 
                if(enemy.isAlive() && (mipos -  supos).magnitude <= StatsInfo.detectionRangePerClass[(int)unit.tipo])                   
                {                                                                                                                       
                    attackActions.Add(createAttackingAction(unit,enemy));                                                               
                    //attackGroup.Remove(unit);
                    break;
                }
            }

        }
        return attackActions;
    }

    private List<Accion> attackEnemyBase()
    {
        List<Accion> attackBaseActions = new List<Accion>();

        foreach(PersonajeBase unit in unitsNotAsigned)
        {
            if(!attackGroup.Contains(unit))                                 //para no volver a mandarla si ya la he mandado
            {
                attackBaseActions.Add(createBaseAttackAction(unit));  
                attackGroup.Add(unit);           
            }
        }

        return attackBaseActions;
    }

    protected internal override void tioMuerto(PersonajeBase tio)
    {
        attackGroup.Remove(tio);
    }

    protected List<Accion> rezagadosAttack()
    {
        List<Accion> accions = new List<Accion>();
        // volver a por los restos
        foreach (PersonajeBase unit in unitsNotAsigned)
        {
            if (unit.currentAction == null)
            {
                accions.Add(createBaseAttackAction(unit));
                //if (!siegeGroup.Contains(unit)) siegeGroup.Add(unit);
            }
        }
        return accions;
    }
}
