using System.Collections;
using System.Collections.Generic;
using UnityEngine;


                    /*AGGRESIVE TACTICAL MODULE*/
public class AggresiveTM : TacticalModule
{

    private List<PersonajeBase> defensiveGroup;
    private List<PersonajeBase> ofensiveGroup;

    private List<PersonajeBase> unitsNotAsigned;                                            //unidades todavia no asignadas a una tarea

    private int BridgeAttacked;

    public AggresiveTM(Vector2 _baseCoords, List<PersonajeBase> _npcs, List<PersonajeBase> _players) : base(_baseCoords, _npcs, _players)
    {
        defensiveGroup = new List<PersonajeBase>();
        ofensiveGroup = new List<PersonajeBase>();
        BridgeAttacked = 1;
    }

    protected internal override List<Accion> getStrategyActions()
    {
        List<Accion> aggresiveActions = new List<Accion>();

        unitsNotAsigned = allies;                                                           //para marcar las unidades ya asignadas
        
                        /*DEFENSA DE LA BASE*/
        if(ourBaseIsUnderAttack())
        {
            List<PersonajeBase> attackers = enemiesOnBase();                                //enemigos que estan atacando la base
            THREAT_VALUE defensePriority = calculateThreat(attackers.Count);                //nivel de peligro
            int defenders = howManyDefendersWeNeed(defensePriority);                        //calculamos cuantos defensores necesitamos
            if(defensiveGroup.Count != defenders)                                           //si necesitamos mas defensores habra que buscarlos
            {
                aggresiveActions = sendAlliesToDefend(defenders - defensiveGroup.Count);    //se iguala porque es lo primero que se hace
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

    private List<Accion> sendAlliesToDefend(int defenders)                          //manda los defensores necesarios
    {
        //comprobar si hay suficientes unidades que no esten peleando para mandarlas
        //sino mandar las que esten mas cerca hasta cumplir los requirimientos
        //o quedarnos sin unidades
        //añadimos a los elegidos al grupo de defensores
        List<Accion> defendActions = new List<Accion>();
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
        foreach(PersonajeBase unit in unitsNotAsigned)
        {
            if(!defensiveGroup.Contains(unit))                                                                          //si no es alguien que deba defender
            {
                foreach(PersonajeBase enemy in enemies)
                {                
                    if((unit.posicion-enemy.posicion).magnitude <= StatsInfo.detectionRangePerClass[(int)unit.tipo])    //distancia de deteccion
                    {
                        unitsNotAsigned.Remove(unit);                                                                   //eliminamos de las unidades a mandar
                        ofensiveGroup.Remove(unit);                                                                     //lo quitamos de grupo de ataque si estaba
                        attackActions.Add(createAttackingAction(unit,enemy));                                           //añadimos accion de atacar
                        break;
                    }
                }
            }
        }
        return attackActions;
    }

}
