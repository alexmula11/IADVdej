

public class WanderSteeringRO : WanderSteering
{
    protected System.Random offsetRandomizer = new System.Random();
    public WanderSteeringRO(float rotationLimit, float offset) : base(rotationLimit, offset)
    {
    }

    protected internal override Steering getSteering(PersonajeBase personaje)
    {
        Steering st = base.getSteering(personaje);
        st.linear = st.linear * (float)offsetRandomizer.NextDouble(); //random offset
        return st;
    }
}
