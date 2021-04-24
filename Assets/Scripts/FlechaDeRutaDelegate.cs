using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlechaDeRutaDelegate : MonoBehaviour
{
    [SerializeField]
    private GameObject flecha, punta;

    internal void setRouteDirection(Vector3 origin, Vector3 end)
    {
        Vector3 route = end - origin;
        transform.position = origin + route / 2;
        transform.eulerAngles = new Vector3(0,SimulationManager.VectorToDirection(route) * Bodi.RadianesAGrados,0);
        flecha.transform.localScale = new Vector3(0.1f, 0.1f, route.magnitude - 2);
        punta.transform.localPosition = new Vector3(0, 0, route.magnitude / 2 - 1);
    }
}
