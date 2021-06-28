using System.Collections;
using System.Collections.Generic;
using UnityEngine;


                    /*AGGRESIVE TACTICAL MODULE*/
public class AggresiveTM : TacticalModule
{

    private const int allBridgedUP = 3;
    private List<PersonajeBase> defensiveGroup = new List<PersonajeBase>();
    private List<PersonajeBase> ofensiveGroup = new List<PersonajeBase>();

    private List<PersonajeBase> unitsNotAsigned = new List<PersonajeBase>();                                            //unidades todavia no asignadas a una tarea

    private int BridgeAttacked;
    private float influenceBridgeSup;
    private float influenceBridgeInf;


    public AggresiveTM(Vector2 _baseCoords,  Vector2 _enemyBaseCoords ,List<PersonajeBase> _npcs, List<PersonajeBase> _players) : base(_baseCoords,_enemyBaseCoords ,_npcs, _players)
    {
        BridgeAttacked = Mathf.RoundToInt(Random.Range(1,2));
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
        else
        {
            unitsNotAsigned.AddRange(defensiveGroup);
            defensiveGroup.Clear();
        }

                        /*COMPROBACION DE ENEMIGOS CERCANOS*/  
        aggresiveActions.AddRange(attackEnemiesClose());

                        /*CONTROL DE UN PUENTE COMO PUNTA DE LANZA*/
        int bridgesControlled = bridgeUnderControl();                                       //comprobamos que puentes controlamos
        if(bridgesControlled == 0)                                                          //si no controlamos ninguno
        {
            //BridgeAttacked = changeBridgeAttack();                                        //selecionamos que puente atacar, vamos intercalando para despistar jeje
            aggresiveActions.AddRange(orderGroupToAttackBridge(BridgeAttacked));            //creamos un grupo de ataque para el puente
        }

                        /*ATAQUE A LA BASE ENEMIGA*/
        if(bridgesControlled != 0)
        {
            Debug.Log("Controlamos el puente: "+bridgesControlled);
            //si no tenemos los dos puentes, comprobamos si el enemigo controla el otro, en ese caso
            //vamos a darle mandanga
            if(bridgesControlled != allBridgedUP)aggresiveActions.AddRange(goToSpearHead(bridgesControlled));

            //despues desde ahi ir a atacar la base
            //en este punto ya solo quedan unidades que no estan haciendo todo lo demas,
            //es decir, gente que esta en el puente esperando y sin luchar,
            //por lo tanto, A POR LA BASE!
            aggresiveActions.AddRange(createSiegeGroup());
        }

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
                if (!defensiveGroup.Contains(ally))
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
            defensiveGroup.Add(unitsNotAsigned[index]);
            unitsNotAsigned.Remove(unitsNotAsigned[index]); //si falla algo, mirar aqui
            priorities[index] = float.MinValue;
        }

        return defendActions;
    }

    private int bridgeUnderControl()              //todo                                                                  //comprobar si controlamos los puentes
    {
        int bridgesControlled = 0;
        bool supControlled = false;
        bool infControlled = false;
        float influenceSup = 0;
        float influenceInf = 0;

        //0 - ninguno
        //1 - el de arriba   
        for(int i = (int)StatsInfo.puente_superior[0].x; i < StatsInfo.puente_superior[2].x; i++)
        {
            for(int j = (int)StatsInfo.puente_superior[0].y; j < StatsInfo.puente_superior[1].y; j++)
            {
                influenceSup+=SimManagerFinal.influences[i][j];
            }
        }
        if(influenceSup<0){
            supControlled = true;
            bridgesControlled = 1;
        }

        //2 - el de abajo
        for(int i = (int)StatsInfo.puente_inferior[0].x; i < StatsInfo.puente_inferior[2].x; i++)
        {
            for(int j = (int)StatsInfo.puente_inferior[0].y; j < StatsInfo.puente_inferior[1].y; j++)
            {
                influenceInf+=SimManagerFinal.influences[i][j];
            }
        }
        if(influenceInf<0){
            infControlled = true;
            bridgesControlled = 2;
        }

        //3 - ambos
        if(supControlled && infControlled) bridgesControlled = 3;

        influenceBridgeSup = influenceSup;
        influenceBridgeInf = influenceInf;

        return bridgesControlled;
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

    private List<Accion> orderGroupToAttackBridge(int bridge)   //TODO revisar referencia coordenadas
    {
        List<Accion> attackActions = new List<Accion>();
        int whereToGoX = 0;
        int whereToGoY=0;
        ActionGo goToBridge = null;
        //crear un grupo de unidades, cercanas entre ellas en el mapa de influencia
        //y mandarlas a por el puente
        foreach(PersonajeBase unit in unitsNotAsigned)
        {
            if(!defensiveGroup.Contains(unit) && !alreadyInBridge(unit,bridge) && !ofensiveGroup.Contains(unit))
            {
                
                switch(bridge)
                {
                    case 1:
                        if(unit.tipo == StatsInfo.TIPO_PERSONAJE.INFANTERIA || unit.tipo == StatsInfo.TIPO_PERSONAJE.PESADA)
                        {
                            whereToGoX = Mathf.RoundToInt(Random.Range(74f,81f));   
                            whereToGoY = Mathf.RoundToInt(Random.Range(19f,24f));
                        }
                        else 
                        {
                            whereToGoX = Mathf.RoundToInt(Random.Range(82f,89f));
                            whereToGoY = Mathf.RoundToInt(Random.Range(19f,24f));
                        }

                        goToBridge = new ActionGo(unit, new Vector2(whereToGoX,whereToGoY),null);
                        ofensiveGroup.Add(unit);
                        attackActions.Add(goToBridge);
                    break;
                    case 2:
                        if(unit.tipo == StatsInfo.TIPO_PERSONAJE.INFANTERIA || unit.tipo == StatsInfo.TIPO_PERSONAJE.PESADA)
                        {
                            whereToGoX = Mathf.RoundToInt(Random.Range(74f,81f));
                            whereToGoY = Mathf.RoundToInt(Random.Range(39f,44f));
                        }
                        else 
                        {
                            whereToGoX = Mathf.RoundToInt(Random.Range(82f,89f));
                            whereToGoY = Mathf.RoundToInt(Random.Range(39f,44f));
                        }
                        goToBridge = new ActionGo(unit, new Vector2(whereToGoX,whereToGoY),null);
                        ofensiveGroup.Add(unit);
                        attackActions.Add(goToBridge);
                    break;
                }
            }
        }
        
        return attackActions;
    }

    private List<Accion> attackEnemiesClose()
    {
        List<Accion> attackActions = new List<Accion>();
        List<PersonajeBase> unidadKataka = new List<PersonajeBase>();
        foreach(PersonajeBase unit in unitsNotAsigned)
        {
            if(!defensiveGroup.Contains(unit))                                                                                              //si no es alguien que deba defender
            {
                foreach(PersonajeBase enemy in enemies)
                {               
                    Vector2 mipos = SimManagerFinal.positionToGrid(unit.posicion);
                    Vector2 supos = SimManagerFinal.positionToGrid(enemy.posicion); 
                    if(enemy.isAlive() && (mipos -  supos).magnitude <= StatsInfo.detectionRangePerClass[(int)unit.tipo])                   //distancia de deteccion
                    {                                                                                                                       //lo quitamos de grupo de ataque si estaba
                        attackActions.Add(createAttackingAction(unit,enemy));                                                                //añadimos accion de atacar
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
            }
        }
        return attackActions;
    }

    private List<Accion> goToSpearHead(int bridgesControlled) //TODO si el otro puenten no es del enemigo, reforzar el nuestro
    {
        List<Accion> regroupForAttack = new List<Accion>();
        float influenceTotAttack = 0;
        int bridgeToAttack = 0;
        float influenceToBeat = 0;
        switch(bridgesControlled)
        {
            //si tengo el 1 ataco al 2
            case 1:
                if(influenceBridgeInf > 0)
                {
                    bridgeToAttack = 2;
                    influenceToBeat = influenceBridgeInf;
                }
            break;
            //si tengo el 2 ataco al 1
            case 2:
                if(influenceBridgeSup > 0)
                {
                    bridgeToAttack = 1;
                    influenceToBeat = influenceBridgeSup;
                }    
            break;
        }

        if(bridgeToAttack != 0)         //si el enemigo tiene un puente tratamos de recuperarlo
        {
            foreach(PersonajeBase npc in unitsNotAsigned)
            {
                if(!defensiveGroup.Contains(npc))
                {
                    if(!alreadyInBridge(npc,bridgesControlled))
                    {
                        if(influenceTotAttack > influenceToBeat)
                        {
                            break;
                        }
                        influenceTotAttack += StatsInfo.influenciaMaximaGeneradaPorUnidad[(int)npc.tipo];
                        if(!alreadyGoingToBridge(npc,bridgeToAttack))
                            regroupForAttack.Add(new ActionGo(npc,randomPointInBridge(bridgeToAttack,npc),null));
                        if(!ofensiveGroup.Contains(npc))
                            ofensiveGroup.Add(npc);
                    }
                }
            }

            if(influenceTotAttack < influenceToBeat)
            {
                foreach(PersonajeBase npc in unitsNotAsigned)
                {
                    if(!defensiveGroup.Contains(npc))
                    {
                        if(influenceTotAttack > influenceToBeat)
                        {
                            break;
                        }
                        influenceTotAttack += StatsInfo.influenciaMaximaGeneradaPorUnidad[(int)npc.tipo];
                        if(!alreadyGoingToBridge(npc,bridgeToAttack))
                            regroupForAttack.Add(new ActionGo(npc,randomPointInBridge(bridgeToAttack,npc),null));
                        if(!ofensiveGroup.Contains(npc))
                            ofensiveGroup.Add(npc);
                    }  
                }
            }

        }
        else                //si no, vamos la que tenemos conquistado
        {
            foreach(PersonajeBase npc in unitsNotAsigned)
            {
                if(!defensiveGroup.Contains(npc))
                {
                    if(!alreadyInBridge(npc,bridgesControlled))
                    {
                        if(!alreadyGoingToBridge(npc,bridgesControlled)){
                            Debug.Log("entro");
                            regroupForAttack.Add(new ActionGo(npc,randomPointInBridge(bridgesControlled,npc),null));
                        }
                            
                        if(!ofensiveGroup.Contains(npc))
                            ofensiveGroup.Add(npc);
                    }
                }
            }
        }

        return regroupForAttack;
    }

    private bool alreadyInBridge(PersonajeBase npc,int whichBridge) //hay que pasarlo 1 o 2 como valor de entrada
    {
        Vector2 pos = SimManagerFinal.positionToGrid(npc.posicion);
        switch(whichBridge)
        {
            case 1:
                return pos.x >= StatsInfo.puente_superior[0].x && pos.x <= StatsInfo.puente_superior[2].x && pos.y >= StatsInfo.puente_superior[0].y && pos.y <= StatsInfo.puente_superior[1].y;
            case 2:
                return pos.x >= StatsInfo.puente_inferior[0].x && pos.x <= StatsInfo.puente_inferior[2].x && pos.y >= StatsInfo.puente_inferior[0].y && pos.y <= StatsInfo.puente_inferior[1].y;
            case 3:
                return
                (pos.x >= StatsInfo.puente_superior[0].x && pos.x <= StatsInfo.puente_superior[2].x && pos.y >= StatsInfo.puente_superior[0].y && pos.y <= StatsInfo.puente_superior[1].y)
                ||
                (pos.x >= StatsInfo.puente_inferior[0].x && pos.x <= StatsInfo.puente_inferior[2].x && pos.y >= StatsInfo.puente_inferior[0].y && pos.y <= StatsInfo.puente_inferior[1].y);
        } 
        return false;
    }

    private List<Accion> createSiegeGroup()
    {
        List<Accion> siegeActions = new List<Accion>();
        return siegeActions;
    }

    private Vector2 randomPointInBridge(int bridge, PersonajeBase unit)
    {
        int whereToGoX = 0;
        int whereToGoY = 0;
        switch(bridge)
        {
            case 1:
                if(unit.tipo == StatsInfo.TIPO_PERSONAJE.INFANTERIA || unit.tipo == StatsInfo.TIPO_PERSONAJE.PESADA)
                {
                    whereToGoX = Mathf.RoundToInt(Random.Range(74f,81f));   
                    whereToGoY = Mathf.RoundToInt(Random.Range(19f,24f));
                }
                else 
                {
                    whereToGoX = Mathf.RoundToInt(Random.Range(82f,89f));
                    whereToGoY = Mathf.RoundToInt(Random.Range(19f,24f));
                }
            break;
            case 2:
                if(unit.tipo == StatsInfo.TIPO_PERSONAJE.INFANTERIA || unit.tipo == StatsInfo.TIPO_PERSONAJE.PESADA)
                {
                    whereToGoX = Mathf.RoundToInt(Random.Range(74f,81f));
                    whereToGoY = Mathf.RoundToInt(Random.Range(39f,44f));
                }
                else 
                {
                    whereToGoX = Mathf.RoundToInt(Random.Range(82f,89f));
                    whereToGoY = Mathf.RoundToInt(Random.Range(39f,44f));
                }
            break;
        }
        return new Vector2(whereToGoX,whereToGoY);
    }


    private bool pointInBridge(Vector2 pos, int whichBridge)
    {
        switch(whichBridge)
        {
            case 1:
                return pos.x >= StatsInfo.puente_superior[0].x && pos.x <= StatsInfo.puente_superior[2].x && pos.y >= StatsInfo.puente_superior[0].y && pos.y <= StatsInfo.puente_superior[1].y;
            case 2:
                return pos.x >= StatsInfo.puente_inferior[0].x && pos.x <= StatsInfo.puente_inferior[2].x && pos.y >= StatsInfo.puente_inferior[0].y && pos.y <= StatsInfo.puente_inferior[1].y;
        } 
        return false;
    }

    private bool alreadyGoingToBridge(PersonajeBase npc,int whichBridge)
    {
        if(npc.currentAction is ActionGo)
        {
            if(pointInBridge(((ActionGo)npc.currentAction).getDestiny(),whichBridge))
            {
                return true;
            }
        }
        return false;
    }
}
