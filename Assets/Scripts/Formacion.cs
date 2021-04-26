using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Formacion : MonoBehaviour
{
    protected List<PersonajeBase> miembros = new List<PersonajeBase>();
    protected List<Vector3> offsetPositions = new List<Vector3>();

    protected PersonajeBase lider { get { return miembros[0]; } }

    protected int maximoMiembros;
    protected int n_miembros =0;

    public Formacion(PersonajeBase lider, int maximoMiembros)
    {
        miembros.Add(lider);
        n_miembros = 1;
        this.maximoMiembros = maximoMiembros;
    }

    internal void addMiembro(PersonajeBase nuevo){
         miembros.Add(nuevo);
         if (n_miembros < maximoMiembros){
            n_miembros ++;
         }
    }

    internal void addMiembros(List<PersonajeBase> nuevos){
         foreach (PersonajeBase nuevo in nuevos)
         {
             if (n_miembros < maximoMiembros){
                addMiembro(nuevo);
             }
         }
    }

    internal abstract void formacionASusPuestos(Vector3 posicionLider);
    internal abstract void formacionEnAccion(Vector3 posicionLider);
}
