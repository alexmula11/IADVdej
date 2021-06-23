using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AccionCombate : Accion
{
    protected internal PersonajeBase receptor;
    public AccionCombate(PersonajeBase _sujeto, PersonajeBase _receptor) : base(_sujeto)
    {
        receptor = _receptor;
        this.nombreAccion = "ATACAR";
    }

    protected internal abstract override void doit();

    protected internal abstract override bool isDone();
    
    protected internal abstract bool isInRange();
    
}
