using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Formacion 
{

    protected Stopwatch stopwatch = new Stopwatch();
    protected List<PersonajeBase> miembros = new List<PersonajeBase>();
    protected List<Vector3> offsetPositions = new List<Vector3>();
    protected List<float> offsetRotations = new List<float>();


    protected internal PersonajeBase lider { get { return miembros[0]; } }
    protected float rotacion { get { return lider.orientacion; } }

    protected int maximoMiembros;
    protected int n_miembros =0;

    public Formacion(PersonajeBase lider, int maximoMiembros)
    {
        miembros.Add(lider);
        n_miembros = 1;
        this.maximoMiembros = maximoMiembros;
        stopwatch.Start();
    }

    internal bool addMiembro(PersonajeBase nuevo){
         if (n_miembros < maximoMiembros)
         {
            miembros.Add(nuevo);
            n_miembros ++;
            return true;
         }
        return false;
    }

    internal bool addMiembros(List<PersonajeBase> nuevos){
        bool todos = true;
         foreach (PersonajeBase nuevo in nuevos)
        {
            todos = addMiembro(nuevo) && todos;
        }
        return todos;
    }

    internal void formacionASusPuestos()
    {
        for (int i = 1; i < n_miembros; i++)
        {

            FormacionSD opSD = new FormacionSD(offsetPositions[i - 1],offsetRotations[i-1]);
            opSD.target = lider;
            miembros[i].newTask(opSD);
        }
    }

    internal void disband()
    {
        for (int i=0; i<miembros.Count; i++)
        {
            miembros[i].disband();
        }
    }

    internal void checkWaitForFormation()
    {
        //Para detener al lider cada 5 segundos
        if (stopwatch.ElapsedMilliseconds > 10000)
        {
            lider.addTask(new WaitSteering(6000));
            stopwatch.Restart();
        }
    }



}
