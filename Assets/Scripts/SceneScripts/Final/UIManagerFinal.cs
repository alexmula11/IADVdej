using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerFinal : UIManager
{
    [SerializeField]
    protected Text debugAgentAction;

    internal new void actualizeAgentDebugInfo(PersonajeBase character)
    {
        debugAgentName.text = "Agent Name: " + character.nick;

        if (character.accion != null) debugAgentName.text = "Selected Action: " + character.accion.GetType().Name;
        else debugAgentName.text = "Selected Behaviour: No action";

        if (character.selectedBehaviour != null) debugAgentBehaviour.text = "Selected Behaviour: " + character.selectedBehaviour.GetType().Name;
        else debugAgentBehaviour.text = "Selected Behaviour: No behaviour";
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
    }
}
