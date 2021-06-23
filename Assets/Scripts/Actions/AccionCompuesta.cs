using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AccionCompuesta : Accion
{
    protected internal int actionIndex = 0;
    public AccionCompuesta(PersonajeBase _sujeto) : base(_sujeto)
    {
    }

    protected internal override void doit()
    {
        throw new System.NotImplementedException();
    }

    protected internal abstract void actualizeIt();
}
