using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    protected List<PersonajePlayer> personajesPlayer = new List<PersonajePlayer>();
    protected List<PersonajeNPC> personajesNPC = new List<PersonajeNPC>();

    protected bool playerIAActive = false;

    protected TacticIA playerIA, enemyIA;


    [SerializeField]
    protected Transform baseAliada, baseEnemiga;

    internal Transform allyBase { get { return baseAliada; } }
    internal Transform enemyBase { get { return baseEnemiga; } }

    protected float iaTimer = 0f;

    protected float blueBase = StatsInfo.MAX_BASE_HEALTH;
    protected float redBase = StatsInfo.MAX_BASE_HEALTH;


    


    protected void FixedUpdate()
    {
        ActualizeBasesHealth();

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

                }
            }
        }

    }

    protected internal void checkWinningCondition()
    {
        if(redBase == 0)
        {
            SimManagerFinal.setWinner(0);
        }
        else
        {
            if(blueBase == 0)
            {
                SimManagerFinal.setWinner(1);
            }
        }
    }

}
