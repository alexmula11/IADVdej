﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private SimulationManager simManager;

    [SerializeField]
    private Image unitSelectionMark;

    [SerializeField]
    private GameObject debugInfo;
    [SerializeField]
    private Text debugAgentName, debugAgentAction, debugAgentLookingDirectionAngle, debugAgentTargetLookingDirectionAngle, debugAgentSteeringLinear, debugAgentSteeringAngular, debugAgentSteeringCombination;

    [SerializeField]
    private MouseOptionDelegate[] mouseButtons;


    private Vector2 selectionOrigin;


    internal void showDebugInfo(bool show)
    {
        debugInfo.SetActive(show);
    }
    internal void actualizeAgentDebugInfo(PersonajeBase character)
    {
        debugAgentName.text = "Agent Name: " + character.nick;
        if (character.selectedBehaviour != null) debugAgentAction.text = "Selected Behaviour: " + character.selectedBehaviour.GetType().Name;
        else debugAgentAction.text = "Selected Behaviour: No behaviour";
        debugAgentLookingDirectionAngle.text = "Looking Direction Angle: " + character.orientacion;
        if (character.steeringActual != null)
        {
            debugAgentSteeringLinear.text = "Linear Steering: " + character.steeringActual.linear;
            debugAgentSteeringAngular.text = "Angular Steering: " + character.steeringActual.angular;
        }
        else
        {
            debugAgentSteeringLinear.text = "Linear Steering: (0, 0, 0)";
            debugAgentSteeringAngular.text = "Angular Steering: 0";
        }
        if ((character.selectedBehaviour as FlockingSD) != null)
        {
            debugAgentSteeringCombination.gameObject.SetActive(true);
            FlockingSD flocking = (character.selectedBehaviour as FlockingSD);
            debugAgentSteeringCombination.text = "Dyn Priority: Ch[" +System.Math.Round(flocking.chPercentDyn,2)
                + "] Sep[" + System.Math.Round(flocking.sepPercentDyn, 2) + "] Foll[" + System.Math.Round(flocking.followLeaderPercentDyn, 2) + "]";
        }
        else
        {
            debugAgentSteeringCombination.gameObject.SetActive(false);
        }
    }





    /*internal void startUnitSelection(Vector2 origin)
    {
        selectionOrigin = origin;
        unitSelectionMark.gameObject.SetActive(true);
        unitSelectionMark.rectTransform.sizeDelta = new Vector2(0, 0);
        unitSelectionMark.rectTransform.anchoredPosition = new Vector2(origin.x,origin.y);
        unitSelectionMark.rectTransform.ForceUpdateRectTransforms();
    }

    internal void trackUnitSelection(Vector2 mousePos)
    {
        unitSelectionMark.rectTransform.anchoredPosition = selectionOrigin + (mousePos - selectionOrigin)/2;
        unitSelectionMark.rectTransform.sizeDelta = new Vector2(System.Math.Abs(mousePos.x - selectionOrigin.x), System.Math.Abs(mousePos.y - selectionOrigin.y));
        unitSelectionMark.rectTransform.ForceUpdateRectTransforms();
    }

    internal void releaseUnitSelection()
    {
        unitSelectionMark.gameObject.SetActive(false);
    }*/




    //UI BUTTONS
    public void selectMouseOption(int pos)
    {
        for (int i=0; i<mouseButtons.Length; i++)
        {
            mouseButtons[i].select(pos==i);
        }
        simManager.setMouseBehaviour(pos);
    }
    public void mouseOverUI(bool over)
    {
        simManager.setMouseOverUI(over);
    }

    internal void actualizeUserButtons(HashSet<StatsInfo.ACCION> posiblesAcciones)
    {
        foreach( MouseOptionDelegate button in mouseButtons)
        {
            button.gameObject.SetActive(false);
        }
        mouseButtons[0].gameObject.SetActive(true);
        foreach (StatsInfo.ACCION accion in posiblesAcciones)
        {
            switch (accion)
            {
                case StatsInfo.ACCION.MOVE_OR_FOLLOW:
                    mouseButtons[1].gameObject.SetActive(true);
                    break;
                case StatsInfo.ACCION.FORMATION:
                    mouseButtons[2].gameObject.SetActive(true);
                    mouseButtons[3].gameObject.SetActive(true);
                    mouseButtons[4].gameObject.SetActive(true);
                    break;
                case StatsInfo.ACCION.ROUTE:
                    mouseButtons[5].gameObject.SetActive(true);
                    break;
            }
        }
    }
}
