using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMatchingSteering : SteeringBehaviour
{

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();

        //st.linear = _target.velocidad - personaje.velocidad; <-- ESTO ES HACERLO POR SUMA DE COMPONENTES
        st.linear = _target.velocidad - personaje.velocidad; // <-- ESTO ES HACERLO POR SUMA DE DESTINO, O LO HACEMOS TODO POR COMPONENTE O TODO POR SUMA DE DESTINO

        if(st.linear.magnitude > personaje.movAcc)
        {
            st.linear = st.linear.normalized * personaje.movAcc;
        }
        if (_target.velocidad == personaje.velocidad)
        {
            _finished = true;
        }
        return st;
    }

}
