using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

//Esta formacion está formada por 4 jugadores
//El lider se ubica en una posicion determinada por el jugador 
//Los 3 otros miembros forman fila detras de el
public class FormacionCuadrado : Formacion
{

    public FormacionCuadrado(PersonajeBase lider) :base(lider, 5){
        for (int i = 1; i < maximoMiembros; i++)
        {
            if (i == 1)
            {
                offsetPositions.Add(new Vector3(-7.5f,0,7.5f));
            }else if (i == 2)
            {
                offsetPositions.Add(new Vector3(7.5f, 0, 7.5f));
            }else if (i == 3)
            {
                offsetPositions.Add(new Vector3(-7.5f, 0, -7.5f));
            }
            else
            {
                offsetPositions.Add(new Vector3(7.5f, 0, -7.5f));
            }
        }
        for (int i = 1; i < maximoMiembros; i++)
        {
            float offsetRotation = SimulationManager.VectorToDirection(offsetPositions[i - 1]);
            if (offsetRotation > Math.PI)
            {
                offsetRotation -= (float)Math.PI * 2;
            }else if (offsetRotation < Math.PI)
            {
                offsetRotation += (float)Math.PI*2;
            }
            offsetRotations.Add(offsetRotation);
        }
    }
}
