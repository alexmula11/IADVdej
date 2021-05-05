using System.Collections;
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
    private Text debugAgentName, debugAgentAction, debugAgentLookingDirectionAngle, debugAgentTargetLookingDirectionAngle, debugAgentSteeringLinear, debugAgentSteeringAngular;

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
        debugAgentAction.text = "Selected Behaviour: " + character.selectedBehaviour.GetType().Name;
        debugAgentLookingDirectionAngle.text = "Looking Direction Angle: " + character.orientacion;
        debugAgentSteeringLinear.text = "Linear Steering: " + character.steeringActual.linear;
        debugAgentSteeringAngular.text = "Angular Steering: " + character.steeringActual.angular;
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
}
