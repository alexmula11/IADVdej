using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Accion
{
    protected string nombreAccion;

    protected internal string nombre { get { return nombreAccion; } }

    protected internal PersonajeBase sujeto;

    public Accion(PersonajeBase _sujeto)
    {
        sujeto = _sujeto;
    }
    
    protected internal abstract void doit();

    protected internal abstract bool isDone();

    protected internal abstract bool isPossible();
}
