using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormacionPorRoles : Formacion
{

    protected ROL[] rolesPorPosicion;
    protected HashSet<StatsInfo.TIPO_PERSONAJE> clasesParaElRol(ROL rol)
    {
        HashSet<StatsInfo.TIPO_PERSONAJE> clases = new HashSet<StatsInfo.TIPO_PERSONAJE>();
        switch (rol)
        {
            //ROLES DUROS
            case ROL.DEFENSOR:
                clases.Add(StatsInfo.TIPO_PERSONAJE.PESADA);
                break;
            case ROL.FLANQUEADOR:
                clases.Add(StatsInfo.TIPO_PERSONAJE.INFANTERIA);
                break;
            case ROL.SNIPER:
                clases.Add(StatsInfo.TIPO_PERSONAJE.ARQUERO);
                break;
            case ROL.CASTER:
                clases.Add(StatsInfo.TIPO_PERSONAJE.MAJITO);
                break;
            //ROLES BLANDOS
            case ROL.MELEE:
                clases.Add(StatsInfo.TIPO_PERSONAJE.PESADA);
                clases.Add(StatsInfo.TIPO_PERSONAJE.INFANTERIA);
                break;
            case ROL.RANGED:
                clases.Add(StatsInfo.TIPO_PERSONAJE.ARQUERO);
                clases.Add(StatsInfo.TIPO_PERSONAJE.MAJITO);
                break;
        }
        return clases;
    }
    protected HashSet<ROL> rolesParaLaClase(StatsInfo.TIPO_PERSONAJE clase)
    {
        HashSet<ROL> roles = new HashSet<ROL>();
        switch (clase)
        {
            case StatsInfo.TIPO_PERSONAJE.INFANTERIA:
                roles.Add(ROL.FLANQUEADOR);
                roles.Add(ROL.MELEE);
                break;
            case StatsInfo.TIPO_PERSONAJE.ARQUERO:
                roles.Add(ROL.SNIPER);
                roles.Add(ROL.RANGED);
                break;
            case StatsInfo.TIPO_PERSONAJE.PESADA:
                roles.Add(ROL.DEFENSOR);
                roles.Add(ROL.MELEE);
                break;
            case StatsInfo.TIPO_PERSONAJE.MAJITO:
                roles.Add(ROL.CASTER);
                roles.Add(ROL.RANGED);
                break;
        }
        return roles;
    }

    public enum ROL
    {
        DEFENSOR,
        FLANQUEADOR,
        SNIPER,
        CASTER,
        MELEE,
        RANGED
    }


    public FormacionPorRoles(PersonajeBase lider) : base(lider, 10)
    {
        rolesPorPosicion = new ROL[maximoMiembros - 1];

        offsetPositions[0] = new Vector3(0, 0, 10);
        offsetRotations[0] = 0;
        rolesPorPosicion[0] = ROL.FLANQUEADOR;

        offsetPositions[1] = new Vector3(-7.5f, 0, 7.5f);
        offsetRotations[1] = -45*Bodi.GradosARadianes;
        rolesPorPosicion[1] = ROL.DEFENSOR;

        offsetPositions[2] = new Vector3(7.5f, 0, 7.5f);
        offsetRotations[2] = 45 * Bodi.GradosARadianes;
        rolesPorPosicion[2] = ROL.DEFENSOR;

        offsetPositions[3] = new Vector3(-7.5f, 0, -7.5f);
        offsetRotations[3] = -90 * Bodi.GradosARadianes;
        rolesPorPosicion[3] = ROL.MELEE;

        offsetPositions[4] = new Vector3(7.5f, 0, -7.5f);
        offsetRotations[4] = 90 * Bodi.GradosARadianes;
        rolesPorPosicion[4] = ROL.MELEE;

        offsetPositions[5] = new Vector3(0, 0, -10);
        offsetRotations[5] = 0;
        rolesPorPosicion[5] = ROL.CASTER;

        offsetPositions[6] = new Vector3(-10, 0, -20);
        offsetRotations[6] = -45 * Bodi.GradosARadianes;
        rolesPorPosicion[6] = ROL.SNIPER;

        offsetPositions[7] = new Vector3(10, 0, -20);
        offsetRotations[7] = 45 * Bodi.GradosARadianes;
        rolesPorPosicion[7] = ROL.SNIPER;

        offsetPositions[8] = new Vector3(0, 0, -20);
        offsetRotations[8] = -180 * Bodi.GradosARadianes;
        rolesPorPosicion[8] = ROL.RANGED;
    }

    internal override bool addMiembro(PersonajeBase miembro)
    {
        for (int i=0; i < rolesPorPosicion.Length; i++)
        {
            if (miembros[i+1]==null)
            {
                //miembro apto para la posicion
                if (clasesParaElRol(rolesPorPosicion[i]).Contains(miembro.tipo))
                {
                    miembros[i+1] = miembro;
                    n_miembros++;
                    return true;
                }
            }
        }
        return false;
    }

    internal new void formacionASusPuestos()
    {
        for (int i = 1; i < miembros.Length; i++)
        {
            if (miembros[i] != null)
            {
                FormacionSD opSD = new FormacionSD(offsetPositions[i - 1], offsetRotations[i - 1]);
                opSD.target = lider;
                miembros[i].newTask(opSD);
            }
        }
    }
}
