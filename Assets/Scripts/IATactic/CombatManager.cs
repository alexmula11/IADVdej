using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager
{
   List <AccionCombate> combatlist;


    //TODO combatAction generica de la cual heredan ataque y curacion

    public CombatManager()
    {
        combatlist = new List<AccionCombate>();
    }

    //Metodo para actualizar los combates activos
    public void manageCombats()
    {
        List<int>combatsFinished = new List<int>();             //lista para guardar los indices de los combates que acaban
        int ind = 0;

        foreach(AccionCombate cbt in combatlist)
        {
            cbt.doit();                                          //realizamos la accion
            if(cbt.isDone())
                combatsFinished.Add(ind);
            ind++;
        }

        foreach(int k in combatsFinished)                        //Eliminamos las acciones marcadas como acabadas
        {
            combatlist.RemoveAt(k);
        }
    }


    //Metodo para agregar acciones de combate
    public void addCombat(AccionCombate act)
    {
        combatlist.Add(act);
    }
    public void addCombat(List<AccionCombate> lact)
    {
        foreach(AccionCombate act in lact)
            combatlist.Add(act);
    }

    public bool isBeingAttacked(PersonajeNPC npc)
    {
        PersonajeBase aggresor = null;
        foreach(AccionAttack att in combatlist)
        {
            aggresor = (att.receptor==npc) ? att.sujeto : null;
            if(aggresor)
                break;
        }
        return aggresor;
    }

}
