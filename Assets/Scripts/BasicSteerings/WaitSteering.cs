using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class WaitSteering : SteeringBehaviour
{
    private Stopwatch stopwatch = new Stopwatch();
    private float milis;


    public WaitSteering(float milis)
    {
        this.milis = milis;
        stopwatch.Start();
    }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        _finishedLinear = _finishedAngular = stopwatch.ElapsedMilliseconds > milis;
        Steering st = new Steering();
        st.linear = -personaje.velocidad;
        return st;
    }
}
