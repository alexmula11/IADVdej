using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Accion
{
    protected internal string nombreAccion;
    protected internal PersonajeBase sujeto;            //sujeto que realiza la accion
    protected internal PersonajeBase receptor;          //sujeto que recibe la accion

    public Accion(string _nombreAccion, PersonajeBase _sujeto, PersonajeBase _receptor)
    {
        nombreAccion = _nombreAccion;
        sujeto = _sujeto;
        receptor = _receptor;
    }
    
    protected internal abstract void doit();
}
