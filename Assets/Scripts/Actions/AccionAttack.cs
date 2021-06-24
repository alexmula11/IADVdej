using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccionAttack : AccionCombate
{


    public AccionAttack(PersonajeBase _sujeto, PersonajeBase _receptor) : base(_sujeto, _receptor)
    {
        this.nombreAccion = "ATACAR";
    }

    protected internal override void doit()
    {
        if(isInRange())                                                                         //si el receptor esta a rango de ataque del sujeto
        {
            receptor.actualizeHealth(-calculateDamageOutput());                                 //le pasamos actualización de vida negativa
        }
    }

    protected internal override bool isDone()
    {
        return (!receptor.isAlive());                                                           //la accion finaliza si a quien atacamos muere
    }

    protected internal override bool isInRange()                                                //Comprobacion de rango de ataque
    {
        return ((sujeto.posicion-receptor.posicion).magnitude 
        <= 
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
