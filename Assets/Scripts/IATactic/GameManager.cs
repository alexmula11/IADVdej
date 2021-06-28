using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    protected HPTeamBarController baseHPBars;

    protected List<PersonajePlayer> personajesPlayer = new List<PersonajePlayer>();
    protected List<PersonajeNPC> personajesNPC = new List<PersonajeNPC>();

    protected bool playerIAActive = false;

    protected TacticIA playerIA, enemyIA;


    [SerializeField]
    protected Transform baseAliada, baseEnemiga;

    internal Transform allyBase { get { return baseAliada; } }
    internal Transform enemyBase { get { return baseEnemiga; } }

    protected float iaTimer = 0f;

    protected float respawnTimer = 10f;

    protected float blueBase = StatsInfo.MAX_BASE_HEALTH;
    protected float redBase = StatsInfo.MAX_BASE_HEALTH;

    [SerializeField]
    protected Transform[] allySpawnPoints, enemySpawnPoints;

    protected static internal LinkedList<PersonajePlayer> muertosAllys = new LinkedList<PersonajePlayer>();
    protected static internal LinkedList<PersonajeNPC> muertosEnemys = new LinkedList<PersonajeNPC>();

    protected static internal void addMuerto(PersonajeBase muerto)
    {
        if (muerto is PersonajePlayer)
        {
            muertosAllys.AddLast(muerto as PersonajePlayer);
        }
        else
        {
            muertosEnemys.AddLast(muerto as PersonajeNPC);
        }
    }


    protected void FixedUpdate()
    {

        ActualizeBasesHealth();

        checkRespawnUnits();

        checkWinningCondition();


        if (iaTimer <= 0)
        {
            if (playerIAActive) playerIA.playIA();
            enemyIA.playIA();
            iaTimer = 3f;
        }
        else
        {
            iaTimer -= Time.fixedDeltaTime;
        }
    }

    protected internal void AddMatraca(PersonajeBase matraca)
    {
        if (matraca is PersonajePlayer)
        {
            personajesPlayer.Add(matraca as PersonajePlayer);
        }
        else
        {
            personajesNPC.Add(matraca as PersonajeNPC);
        }
    }
    protected internal void initIAs()
    {
        playerIA = new TacticIA(new List<PersonajeBase>(personajesPlayer), new List<PersonajeBase>(personajesNPC), SimManagerFinal.positionToGrid(baseAliada.position), SimManagerFinal.positionToGrid(baseEnemiga.position));
        enemyIA = new TacticIA(new List<PersonajeBase>(personajesNPC), new List<PersonajeBase>(personajesPlayer), SimManagerFinal.positionToGrid(baseEnemiga.position), SimManagerFinal.positionToGrid(baseAliada.position));
    }

    protected internal void ActualizeBasesHealth()
    {
        foreach(PersonajeBase unit in personajesPlayer)
        {
            if(unit.isAlive())
            {
                if(TacticalModule.isInBaseRange(unit,SimManagerFinal.positionToGrid(enemyBase.position)))
                {
                    redBase -= StatsInfo.potenciaInfluenciaUnidades[(int)unit.tipo] * Time.fixedDeltaTime;
                }
            }
        }
        foreach(PersonajeBase unit in personajesNPC)
        {
            if(unit.isAlive())
            {
                if(TacticalModule.isInBaseRange(unit,SimManagerFinal.positionToGrid(allyBase.position)))
                {
                    blueBase -= StatsInfo.potenciaInfluenciaUnidades[(int)unit.tipo]  * Time.fixedDeltaTime;
                }
            }
        }
        baseHPBars.actualizeHP(1, redBase / StatsInfo.MAX_BASE_HEALTH);
        baseHPBars.actualizeHP(0, blueBase / StatsInfo.MAX_BASE_HEALTH);
    }

    protected internal void checkWinningCondition()
    {
        if(redBase == 0)
        {
            SimManagerFinal.setWinner(0);
        }
        else if(blueBase == 0) {
            SimManagerFinal.setWinner(1);
        }
    }

    protected void checkRespawnUnits()
    {
        if (respawnTimer <= 0)
        {
            if (muertosAllys.Count > 0)
            {
                PersonajePlayer person = muertosAllys.First.Value;
                muertosAllys.RemoveFirst();
                person.revive(allySpawnPoints[0].position);
                if (muertosAllys.Count > 0)
                {
                    PersonajePlayer person2 = muertosAllys.First.Value;
                    muertosAllys.RemoveFirst();
                    person2.revive(allySpawnPoints[1].position);
                }
            }
            if (muertosEnemys.Count > 0)
            {
                PersonajeNPC person = muertosEnemys.First.Value;
                muertosEnemys.RemoveFirst();
                person.revive(enemySpawnPoints[0].position);
                if (muertosEnemys.Count > 0)
                {
                    PersonajeNPC person2 = muertosEnemys.First.Value;
                    muertosEnemys.RemoveFirst();
                    person2.revive(enemySpawnPoints[1].position);
                }
            }
            respawnTimer = 10f;
        }
        else
        {
            respawnTimer -= Time.fixedDeltaTime;
        }
    }
    

}
