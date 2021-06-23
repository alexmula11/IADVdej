using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFormation : Accion
{
    public ActionFormation(PersonajeBase _sujeto) : base(_sujeto)
    {
    }

    protected internal override void doit()
    {
        throw new System.NotImplementedException();
    }

    protected internal override bool isDone()
    {
        return false;
    }
}
