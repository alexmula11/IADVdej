using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFormation : Accion
{
    FormacionGridSD formacionSD;


    public ActionFormation(PersonajeBase _sujeto, PersonajeBase lider, Vector3 offsetPos, float offsetRotation) : base(_sujeto)
    {
        nombreAccion = "FORMAR";
        formacionSD = new FormacionGridSD(offsetPos, offsetRotation);
        formacionSD.target = lider;
    }

    protected internal override void doit()
    {
        sujeto.newTaskGrid(formacionSD);
    }

    protected internal override bool isDone()
    {
        return false;
    }

    protected internal override bool isPossible()
    {
        return sujeto.isAlive() && formacionSD.target.isAlive();
    }
}
