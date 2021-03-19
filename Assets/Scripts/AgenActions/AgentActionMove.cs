using UnityEngine;

public class AgentActionMove : AgentAction
{
    private Vector2 movementTarget;
    private float arrivingLimit;
    private float deccelLimit;
    internal Vector2 target { get { return movementTarget; } }
    internal float arriveLimit { get { return arrivingLimit; } }
    internal float accLimit { get { return deccelLimit; } }

    public AgentActionMove(Vector2 movementTarget, float arrivingLimit, float deccelLimit)
    {
        accion = ACTION.MOVE;
        this.movementTarget = movementTarget;
        this.arrivingLimit = arrivingLimit;
        this.deccelLimit = deccelLimit;
    }
}
