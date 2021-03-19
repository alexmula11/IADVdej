
using UnityEngine;

public class WanderSteering : SteeringBehaviour
{
    protected float rotationLimit;
    protected float offset;
    protected System.Random randomizer = new System.Random(); //Hay que inicializar el random aquí, si no no es true random
    public WanderSteering(float rotationLimit, float offset)
    {
        this.rotationLimit = rotationLimit;
        this.offset = offset;
    }

    internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = new Steering();
        float angularVariation = (float)randomizer.NextDouble() * rotationLimit - rotationLimit / 2; //rotacion random entre orientacion - limite/2 y orientacion+limite/2
        float nuevoAngulo = personaje.orientacion + angularVariation;
        st.angular = SimulationManager.TurnAmountInDirection(personaje.orientacion,SimulationManager.VectorToDirection(personaje.velocidad)); //rotacion random entre orientacion - limite/2 y orientacion+limite/2

        st.linear = SimulationManager.DirectionToVector(nuevoAngulo) * offset; 
        return st;
    }
}
