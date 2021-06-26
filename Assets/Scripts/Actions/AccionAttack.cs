﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccionAttack : AccionCombate
{
    float timer;
    public AccionAttack(PersonajeBase _sujeto, PersonajeBase _receptor) : base(_sujeto, _receptor)
    {
        this.nombreAccion = "ATACAR";
        timer = 1 / StatsInfo.velocidadDeAtaquePorUnidad[(int)sujeto.tipo];
    }

    protected internal override void doit()
    {
        if(isInRange())                                                                         //si el receptor esta a rango de ataque del sujeto
        {
            sujeto.newTask(new StopSteering());
            if (timer <= 0)
            {
                receptor.actualizeHealth(-calculateDamageOutput());                                 //le pasamos actualización de vida negativa
                timer = 1 / StatsInfo.velocidadDeAtaquePorUnidad[(int)sujeto.tipo];
            }
            else
            {
                timer -= Time.fixedDeltaTime;
            }
        }
        else
        {
            timer = 1 / StatsInfo.velocidadDeAtaquePorUnidad[(int)sujeto.tipo];
        }
        
    }

    protected internal override bool isDone()
    {
        return (!receptor.isAlive());                                                           //la accion finaliza si a quien atacamos muere
    }

    protected internal override bool isInRange()                                                //Comprobacion de rango de ataque
    {
        return ((sujeto.posicion-receptor.posicion).magnitude 
        < 
        StatsInfo.attackRangePerClass[(int)sujeto.tipo]);
    }

    protected internal override bool isPossible()
    {
        return sujeto.isAlive() && receptor.isAlive() && isInRange();
    }

    private float calculateDamageOutput()
    {
        float modifier = StatsInfo.damageModifiers[(int)sujeto.tipo][(int)receptor.tipo];       //modificador de daño
        float damage = StatsInfo.damagePerClass[(int)sujeto.tipo];                              //daño base
        return damage*modifier;
    }
}
