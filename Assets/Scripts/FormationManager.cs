using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationManager : MonoBehaviour
{
    private List<Formacion> formaciones = new List<Formacion>();

    internal void addFormation(Formacion formacion)
    {
        formaciones.Add(formacion);
    }
    internal void removeFormacion(Formacion formacion)
    {
        formaciones.Remove(formacion);
    }

    private void FixedUpdate()
    {
        for (int i=0; i < formaciones.Count; i++)
        {
            formaciones[i].checkWaitForFormation();
        }
    }
}
