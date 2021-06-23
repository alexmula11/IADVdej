using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//SE MANTIENE EL SUJETO-RECEPTOR por si aparecen unidades CURANDERAS

public class AccionHealing : AccionCombate
{
      public AccionHealing(PersonajeBase _sujeto, PersonajeBase _receptor) : base(_sujeto, _receptor)
    {
        this.nombreAccion = "CURAR";
    }

    protected internal override void doit()
    {
        if(isInRange())                                                                         //si el receptor esta a rango de ataque del sujeto
        {
            receptor.actualizeHealth(calculateHealingOutput());                                 //le pasamos actualización de vida positiva
        }
    }

    protected internal override bool isDone()
    {
        return (isInRange());                                                                   //la accion finaliza si la unidad sale de rango de la base
    }

    protected internal override bool isInRange()                                                //Comprobacion del rango de curacion
    {
        //TODO
        return false;
    }

    private float calculateHealingOutput()
    {
        return StatsInfo.BASEHEALING;
    }
}
