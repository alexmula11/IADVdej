using System.Collections;
using System.Collections.Generic;
using UnityEngine;


                    /*AGGRESIVE TACTICAL MODULE*/
public class AggresiveTM : TacticalModule
{

    private List<PersonajeBase> defensiveGroup = new List<PersonajeBase>();
    private List<PersonajeBase> ofensiveGroup = new List<PersonajeBase>();

    private List<PersonajeBase> unitsNotAsigned = new List<PersonajeBase>();                                            //unidades todavia no asignadas a una tarea

    private int BridgeAttacked;

    public AggresiveTM(Vector2 _baseCoords, List<PersonajeBase> _npcs, List<PersonajeBase> _players) : base(_baseCoords, _npcs, _players)
    {
        BridgeAttacked = 1;
    }

    protected internal override List<Accion> getStrategyActions()
    {
        List<Accion> aggresiveActions = new List<Accion>();


        checkAlliesDownAndFill();                                                           //actualizamos los grupos formados con las bajas producidas por el enemigo
                                                                                            //y rellenamos la lista de unidades a las cuales podemos asignar ordenes
        
                        /*DEFENSA DE LA BASE*/
        if(ourBaseIsUnderAttack())
        {
            List<PersonajeBase> attackers = enemiesOnBase();                                //enemigos que estan atacando la base
            THREAT_VALUE defensePriority = calculateThreat(attackers.Count);                //nivel de peligro
            int defenders = howManyDefendersWeNeed(defensePriority);                        //calculamos cuantos defensores necesitamos
            if(defensiveGroup.Count != defenders)                                           //si necesitamos mas defensores habra que buscarlos
            {
                aggresiveActions = sendAlliesToDefend(defenders - defensiveGroup.Count, attackers);    //se iguala porque es lo primero que se hace
            }
        }

                        /*COMPROBACION DE ENEMIGOS CERCANOS*/
        
        aggresiveActions.AddRange(attackEnemiesClose());

                        /*CONTROL DE UN PUENTE COMO PUNTA DE LANZA*/
        int bridgesControlled = bridgeUnderControl();                                       //comprobamos que puentes controlamos
        if(bridgesControlled == 0)                                                          //si no controlamos ninguno
        {
            BridgeAttacked = changeBridgeAttack();                                          //selecionamos que puente atacar, vamos intercalando para despistar jeje
            aggresiveActions.AddRange(orderGroupToAttackBridge(BridgeAttacked));            //creamos un grupo de ataque para el puente
        }

                        /*ATAQUE A LA BASE ENEMIGA*/

        return aggresiveActions;
    }

    private void checkAlliesDownAndFill()
    {
        unitsNotAsigned.Clear();
        foreach(PersonajeBase ally in allies)
        {
            if(!ally.isAlive())
            {
                defensiveGroup.Remove(ally);
                ofensiveGroup.Remove(ally);
            }
            else
            {
                if (!defensiveGroup.Contains(ally) && !ofensiveGroup.Contains(ally))
                    unitsNotAsigned.Add(ally);
            }
        }
    }

    private THREAT_VALUE calculateThreat(int numberAttackers)
    {
        if(numberAttackers <= LOWDANGER)
        {
            return THREAT_VALUE.LOW;
        }else{
            if(numberAttackers <= MIDDANGER){
                return THREAT_VALUE.MEDIUM;
            }else{
                return THREAT_VALUE.HIGH;
            }
        }     
    }

    private int howManyDefendersWeNeed(THREAT_VALUE tv)
    {
        int defs = 0;
        switch(tv)
        {
            case THREAT_VALUE.LOW:
                defs = 5;
            break;
            case THREAT_VALUE.MEDIUM:
                defs = 10;
            break;
            case THREAT_VALUE.HIGH:
                defs = allies.Count;
            break;
        }
        return defs;
    }

    private List<Accion> sendAlliesToDefend(int defenders, List<PersonajeBase> attackers)                          //manda los defensores necesarios
    {
        //comprobar si hay suficientes unidades que no esten peleando para mandarlas
        //sino mandar las que esten mas cerca hasta cumplir los requirimientos
        //o quedarnos sin unidades
        //añadimos a los elegidos al grupo de defensores

        List<Accion> defendActions = new List<Accion>();
        float[] priorities = new float[unitsNotAsigned.Count];
        float distancePriority = 1;
        float velocityPriotiry = 10;
        float dmgPriority = 5;
        for (int i=0; i<unitsNotAsigned.Count; i++)
        {
            priorities[i] = -getClosestPointToBase(unitsNotAsigned[i], baseCoords).magnitude * distancePriority;
            priorities[i] += unitsNotAsigned[i].maxMovSpeed * velocityPriotiry;
            float dmgPower = 0;
            foreach (PersonajeBase person in attackers)
                dmgPower += StatsInfo.damageModifiers[(int)unitsNotAsigned[i].tipo][(int)person.tipo];
            priorities[i] += dmgPower*dmgPriority;
        }

        for (int i=0; i<defenders; i++)
        {
            float higherPriority = float.MinValue;
            int index = 0;
            for (int j = 0; j < priorities.Length; j++)
            {
                if (priorities[j] > higherPriority)
                {
                    higherPriority = priorities[j];
                    index = j;
                }
            }
            defendActions.Add(createAttackingAction(unitsNotAsigned[index], getClosestEnemy(unitsNotAsigned[index], attackers)));
            priorities[index] = float.MinValue;
        }

        return defendActions;
    }

    private int bridgeUnderControl()                                               //comprobar si controlamos los puentes
    {
        //0 - ninguno
        //1 - el de arriba
        //2 - el de abajo
        //3 - ambos
        return 0;
    }

    private int changeBridgeAttack()
    {
        if(BridgeAttacked == 1)
        {
            return 2;
        }
        else{
            return 1;
        }
    }

    private List<Accion> orderGroupToAttackBridge(int bridge)
    {
        //crear un grupo de unidades, cercanas entre ellas en el mapa de influencia
        //y mandarlas a por el puente
        List<Accion> attackActions = new List<Accion>();
        return attackActions;
    }

    private List<Accion> attackEnemiesClose()
    {
        List<Accion> attackActions = new List<Accion>();
        List<PersonajeBase> unidadKataka = new List<PersonajeBase>();
        foreach(PersonajeBase unit in unitsNotAsigned)
        {
            if(!defensiveGroup.Contains(unit))                                                                          //si no es alguien que deba defender
            {
                foreach(PersonajeBase enemy in enemies)
                {                
                    if((unit.posicion-enemy.posicion).magnitude <= StatsInfo.detectionRangePerClass[(int)unit.tipo])    //distancia de deteccion
                    {                                                                                                   //lo quitamos de grupo de ataque si estaba
                        attackActions.Add(createAttackingAction(unit,enemy));                                           //añadimos accion de atacar
                        unidadKataka.Add(unit);
                        break;
                    }
                }
            }
        }
        if (unidadKataka.Count > 0)
        {
            foreach (PersonajeBase person in unidadKataka)
            {
                unitsNotAsigned.Remove(person);                                                                         //eliminamos de las unidades a mandar
                ofensiveGroup.Remove(person);
                defensiveGroup.Add(person);
            }
        }
        return attackActions;
    }

}
