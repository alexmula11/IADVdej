using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Formacion 
{

    protected Stopwatch stopwatch = new Stopwatch();
    protected PersonajeBase[] miembros;
    protected Vector3[] offsetPositions;
    protected float[] offsetRotations;

    protected internal PersonajeBase[] getMiembros { get { return miembros; } }
    protected internal Vector3[] getOffsetsPos { get { return offsetPositions; } }
    protected internal float[] getOffsetsRots { get { return offsetRotations; } }

    protected internal PersonajeBase lider { get { return miembros[0]; } }
    protected float rotacion { get { return lider.orientacion; } }

    protected int maximoMiembros;
    protected int n_miembros =0;

    public Formacion(PersonajeBase lider, int maximoMiembros)
    {
        n_miembros = 1;
        this.maximoMiembros = maximoMiembros;
        miembros = new PersonajeBase[maximoMiembros];
        miembros[0] = lider;
        offsetPositions = new Vector3[maximoMiembros - 1];
        offsetRotations = new float[maximoMiembros - 1];
        stopwatch.Start();
    }



    internal virtual bool addMiembro(PersonajeBase nuevo){
         if (n_miembros < maximoMiembros)
         {
            miembros[n_miembros] = nuevo;
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
        for (int i = 1; i < maximoMiembros; i++)
        {
            if (miembros[i] != null)
            {
                FormacionSD opSD = new FormacionSD(offsetPositions[i - 1], offsetRotations[i - 1]);
                opSD.target = lider;
                miembros[i].newTask(opSD);
            }
        }
    }

    internal void formacionASusPuestosGrid()
    {
        for (int i = 1; i < maximoMiembros; i++)
        {
            if (miembros[i] != null)
            {
                Vector2 start = SimManagerFinal.positionToGrid(offsetPositions[i - 1]);
                FormacionGridSD opSD = new FormacionGridSD(offsetPositions[i - 1], offsetRotations[i - 1]);
                opSD.target = lider;
                miembros[i].newTask(opSD);
            }
        }
    }

    internal void formacionASusPuestosAccion()
    {
        for (int i = 1; i < maximoMiembros; i++)
        {
            if (miembros[i] != null)
            {
                ActionFormation formacionAccion = new ActionFormation(miembros[i],lider, offsetPositions[i - 1], offsetRotations[i - 1]);
                miembros[i].accion = formacionAccion;
                miembros[i].accion.doit();
            }
        }
    }

    internal void disband()
    {
        for (int i=0; i<miembros.Length; i++)
        {
            if (miembros[i] != null)
            {
                miembros[i].disband();
            }
        }
    }

    internal void disbandGrid()
    {
        for (int i = 0; i < miembros.Length; i++)
        {
            if (miembros[i] != null)
            {
                miembros[i].disbandAccion();
            }
        }
    }

    internal void checkWaitForFormation()
    {
        //Para detener al lider cada 5 segundos
        if (stopwatch.ElapsedMilliseconds > 10000)
        {
            lider.addTask(new WaitSteering(3000));
            stopwatch.Restart();
        }
    }



}
