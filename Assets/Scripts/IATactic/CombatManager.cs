using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager
{
   List <Accion> combatlist;


    public CombatManager()
    {
        combatlist = new List<Accion>();
    }

    //Metodo para actualizar los combates activos
    public void manageCombats()
    {
        List<int>combatsFinished = new List<int>();     //lista para guardar los indices de los combates que acaban

        foreach(Accion cbt in combatlist)
        {

        }
    }


    //Metodo para agregar acciones de combate
    public void addCombat(Accion act)
    {
        combatlist.Add(act);
    }
    public void addCombat(List<Accion> lact)
    {
        foreach(Accion act in lact)
            combatlist.Add(act);
    }


}
