using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMatchingSteering : SteeringBehaviour
{
    protected new bool finishedAngular { get { return true; } }
    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        _finishedLinear = false;
        Steering st = new Steering();

        //st.linear = _target.velocidad - personaje.velocidad; <-- ESTO ES HACERLO POR SUMA DE COMPONENTES
        st.linear = _target.velocidad - personaje.velocidad; // <-- ESTO ES HACERLO POR SUMA DE DESTINO, O LO HACEMOS TODO POR COMPONENTE O TODO POR SUMA DE DESTINO

        if(st.linear.magnitude > personaje.movAcc)
        {
            st.linear = st.linear.normalized * personaje.movAcc;
        }
        if (_target.velocidad == personaje.velocidad)
        {
            _finishedLinear = true;
        }
        return st;
    }

}
